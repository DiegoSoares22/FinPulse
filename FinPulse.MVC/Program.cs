using FinPulse.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Habilita suporte a Session (para guardar o Token JWT no navegador do usuário)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ========================================================
// ⚡ REGISTRO DOS CLIENTES TIPADOS COM HTTPCLIENTFACTORY
// ========================================================
// ⚠️ Coloque a porta HTTPS exata em que a sua API roda (ex: https://localhost:7204/ ou https://localhost:7000/)
var apiBaseUrl = "https://localhost:7204/";

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ICategoriaService, CategoriaService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Ativa as sessões

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categorias}/{action=Index}/{id?}");

app.Run();