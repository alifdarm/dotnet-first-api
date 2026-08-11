using Microsoft.Extensions.Options;
using MyFirstApi.Api.Options;
using MyFirstApi.Application.Abstractions;
using MyFirstApi.Application.Contracts;

namespace MyFirstApi.Infrastructure.Services;

public sealed class WeatherService(IOptions<WeatherOptions> options) : IWeatherService
{
    private readonly WeatherOptions _weatherOptions = options.Value;

    public Task<IReadOnlyCollection<WeatherForecastResponse>> GetForecastAsync(CancellationToken cancellationToken)
    {
        var summaries = _weatherOptions.Summaries.Length == 0
            ? ["Mild"]
            : _weatherOptions.Summaries;

        var daysAhead = Math.Max(1, _weatherOptions.DaysAhead);
        var minTemp = _weatherOptions.MinTemperatureC;
        var maxTemp = Math.Max(minTemp + 1, _weatherOptions.MaxTemperatureC);

        var forecast = Enumerable.Range(1, daysAhead)
            .Select(day => new WeatherForecastResponse(
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(day)),
                Random.Shared.Next(minTemp, maxTemp),
                summaries[Random.Shared.Next(summaries.Length)]))
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<WeatherForecastResponse>>(forecast);
    }
}
