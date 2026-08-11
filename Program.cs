using MyFirstApi.Api.Endpoints;
using MyFirstApi.Api.Options;
using MyFirstApi.Application;
using MyFirstApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.Configure<WeatherOptions>(
    builder.Configuration.GetSection(WeatherOptions.SectionName));

builder.Services
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

Console.WriteLine($"Application started in {app.Environment.EnvironmentName} environment.");

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Development environment detected. Enabling OpenAPI documentation.");
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapWeatherEndpoints();
app.MapTodoEndpoints();

app.Run();
