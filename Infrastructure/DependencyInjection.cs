using MyFirstApi.Application.Abstractions;
using MyFirstApi.Infrastructure.Repositories;
using MyFirstApi.Infrastructure.Services;

namespace MyFirstApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
        services.AddScoped<IWeatherService, WeatherService>();
        return services;
    }
}
