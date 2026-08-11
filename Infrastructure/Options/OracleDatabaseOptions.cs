namespace MyFirstApi.Infrastructure.Options;

public sealed class OracleDatabaseOptions
{
    public const string SectionName = "OracleDatabase";

    public string ConnectionString { get; set; } = string.Empty;

    public string Schema { get; set; } = string.Empty;

    public string TableName { get; set; } = "TODOS";
}
