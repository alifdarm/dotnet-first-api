namespace MyFirstApi.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
            context.Response.Headers.Append(HeaderName, correlationId.ToString());
        }

        context.Items["CorrelationId"] = correlationId.ToString();
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(HeaderName))
            {
                context.Response.Headers.Append(HeaderName, correlationId.ToString());
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
