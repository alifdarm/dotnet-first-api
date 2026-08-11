using MyFirstApi.Application.Abstractions;
using MyFirstApi.Infrastructure.Data;
using MyFirstApi.Infrastructure.Options;
using MyFirstApi.Infrastructure.Repositories;
using MyFirstApi.Infrastructure.Services;

namespace MyFirstApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var oracleSection = configuration.GetSection(OracleDatabaseOptions.SectionName);
        services.Configure<OracleDatabaseOptions>(oracleSection);

        var oracleConnectionString = oracleSection.GetValue<string>(nameof(OracleDatabaseOptions.ConnectionString));

        if (string.IsNullOrWhiteSpace(oracleConnectionString))
        {
            services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
        }
        else
        {
            services.AddSingleton<IOracleConnectionFactory, OracleConnectionFactory>();
            services.AddScoped<ITodoRepository, OracleTodoRepository>();
        }

        services.AddScoped<IWeatherService, WeatherService>();
        return services;
    }
}
