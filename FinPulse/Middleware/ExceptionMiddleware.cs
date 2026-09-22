using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FinPulse.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Passa a requisição para o próximo componente do pipeline (o Controller)
            await _next(context);
        }
        catch (Exception ex)
        {
            // Se qualquer exceção estourar em qualquer Controller, cai aqui!
            _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Padrão RFC 7807 (ProblemDetails)
        var response = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "Ocorreu um erro interno no servidor.",
            Detail = _env.IsDevelopment() ? exception.StackTrace : "Entre em contato com o suporte do FinPulse.",
            Instance = context.Request.Path
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}