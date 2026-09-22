using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FinPulse.Context;
using FinPulse.Filters;
using FinPulse.Mappings;
using FinPulse.Middleware;
using FinPulse.Models;
using FinPulse.Repositories;
using FinPulse.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext e Cache
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddMemoryCache(); // ⚡ Cache em memória RAM

// 2. ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. Autenticação JWT Bearer
var secretKey = builder.Configuration["JWT:SecretKey"] ??
    throw new ArgumentException("Chave secreta JWT inválida!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// 4. CORS (Para o Frontend React)
const string OrigensFinPulse = "OrigensFinPulse";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: OrigensFinPulse, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 5. Rate Limiting (Proteção contra abusos)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 20,
                QueueLimit = 0,
                Window = TimeSpan.FromSeconds(10)
            }));

    options.AddFixedWindowLimiter(policyName: "loginLimit", limiterOptions =>
    {
        limiterOptions.AutoReplenishment = true;
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

// 6. Versionamento da API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("api-version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 7. Injeção de Dependências
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddAutoMapper(typeof(FinPulseMappingProfile));

// 8. Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiLoggingFilter>();
})
.AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// 9. Swagger com suporte a XML Comments, Versionamento e Bearer Token Automático
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FinPulse API - v1",
        Version = "v1",
        Description = "API de Gestão Financeira Pessoal FinPulse",
        Contact = new OpenApiContact
        {
            Name = "Diego - Desenvolvedor Full-Stack",
            Email = "diego@finpulse.com"
        }
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "FinPulse API - v2",
        Version = "v2",
        Description = "API FinPulse Versão 2.0 (Otimizada)"
    });

    // ✨ Proteção: Inclui comentários XML apenas se o arquivo existir
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // ✨ Configuração moderna: Swagger anexa 'Bearer ' sozinho
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole apenas o seu Token JWT aqui."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ========================================================
// 10. PIPELINE HTTP
// ========================================================
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FinPulse API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "FinPulse API v2");
    });
}

app.UseHttpsRedirection();

app.UseCors(OrigensFinPulse);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();