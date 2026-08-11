using Oracle.ManagedDataAccess.Client;

namespace MyFirstApi.Infrastructure.Data;

public interface IOracleConnectionFactory
{
    OracleConnection CreateConnection();
}
