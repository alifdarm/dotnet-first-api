using System.Net;

namespace MyFirstApi.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;

            _logger.LogError(ex, "Unhandled exception. CorrelationId={CorrelationId}", correlationId);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                title = ex.GetType().Name,
                status = context.Response.StatusCode,
                detail = ex.Message,
                correlationId
            };

            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}
