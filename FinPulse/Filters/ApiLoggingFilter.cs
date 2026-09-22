using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinPulse.Filters;

public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger;
    private Stopwatch? _stopwatch;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
        _logger = logger;
    }

    // 1. Executa ANTES da Action do Controller ser chamada
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("### [INÍCIO] Executando Action: {Action} em {Controller} | Hora: {Data} ###",
            context.ActionDescriptor.DisplayName,
            context.Controller.GetType().Name,
            DateTime.Now.ToLongTimeString());
    }

    // 2. Executa DEPOIS que a Action foi executada
    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch?.Stop();
        _logger.LogInformation("### [FIM] Action finalizada com Status: {StatusCode} | Tempo: {Elapsed}ms ###",
            context.HttpContext.Response.StatusCode,
            _stopwatch?.ElapsedMilliseconds);
    }
}