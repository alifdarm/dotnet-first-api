using Microsoft.Extensions.Options;
using MyFirstApi.Infrastructure.Options;
using Oracle.ManagedDataAccess.Client;

namespace MyFirstApi.Infrastructure.Data;

public sealed class OracleConnectionFactory(IOptions<OracleDatabaseOptions> options) : IOracleConnectionFactory
{
    private readonly OracleDatabaseOptions _options = options.Value;

    public OracleConnection CreateConnection()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("Oracle connection string is not configured.");
        }

        return new OracleConnection(_options.ConnectionString);
    }
}
