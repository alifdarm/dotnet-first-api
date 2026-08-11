namespace MyFirstApi.Api.Options;

public sealed class WeatherOptions
{
    public const string SectionName = "Weather";

    public int DaysAhead { get; set; } = 5;

    public int MinTemperatureC { get; set; } = -20;

    public int MaxTemperatureC { get; set; } = 55;

    public string[] Summaries { get; set; } =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
}
