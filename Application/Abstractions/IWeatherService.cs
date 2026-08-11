using MyFirstApi.Application.Contracts;

namespace MyFirstApi.Application.Abstractions;

public interface IWeatherService
{
    Task<IReadOnlyCollection<WeatherForecastResponse>> GetForecastAsync(CancellationToken cancellationToken);
}
