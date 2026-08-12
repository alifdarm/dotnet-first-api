namespace MyFirstApi.Api.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? requestId;

        _logger.LogInformation(
            "[INFO] HTTP {Method} {Path} started. CorrelationId={CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        var start = DateTime.UtcNow;

        try
        {
            await _next(context);
        }
        finally
        {
            var elapsedMs = (DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogInformation(
                "[INFO] HTTP {Method} {Path} completed in {ElapsedMs} ms with status {StatusCode}. CorrelationId={CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                elapsedMs,
                context.Response.StatusCode,
                correlationId);
        }
    }
}
