using MyFirstApi.Application.Abstractions;
using MyFirstApi.Application.Services;

namespace MyFirstApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        return services;
    }
}
