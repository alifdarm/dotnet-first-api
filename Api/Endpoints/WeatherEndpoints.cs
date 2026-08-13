using MyFirstApi.Application.Abstractions;

namespace MyFirstApi.Api.Endpoints;

public static class WeatherEndpoints
{
    public static IEndpointRouteBuilder MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/weather")
            .WithTags("Weather");


        group.MapGet("/forecast", async (IWeatherService weatherService, CancellationToken cancellationToken) =>
        {
            Console.WriteLine("Mapping weather forecast endpoint.");
            return Results.Ok(await weatherService.GetForecastAsync(cancellationToken));
        })
            .WithName("GetWeatherForecast");

        group.MapFallback(async context =>
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("The requested resource was not found.");
        })
            .WithName("WeatherFallback");

        return app;
    }
}
