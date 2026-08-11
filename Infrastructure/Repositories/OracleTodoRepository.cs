using Microsoft.Extensions.Options;
using MyFirstApi.Application.Abstractions;
using MyFirstApi.Domain.Entities;
using MyFirstApi.Infrastructure.Data;
using MyFirstApi.Infrastructure.Options;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MyFirstApi.Infrastructure.Repositories;

public sealed class OracleTodoRepository(
    IOracleConnectionFactory connectionFactory,
    IOptions<OracleDatabaseOptions> options) : ITodoRepository
{
    private readonly IOracleConnectionFactory _connectionFactory = connectionFactory;
    private readonly string _qualifiedTableName = BuildQualifiedTableName(options.Value);

    public async Task<IReadOnlyCollection<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = $"SELECT ID, TITLE, IS_COMPLETED FROM {_qualifiedTableName} ORDER BY ID";

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var todos = new List<TodoItem>();

        while (await reader.ReadAsync(cancellationToken))
        {
            todos.Add(MapTodo(reader));
        }

        return todos;
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = $"SELECT ID, TITLE, IS_COMPLETED FROM {_qualifiedTableName} WHERE ID = :id";
        command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });

        using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapTodo(reader) : null;
    }

    public async Task<TodoItem> AddAsync(string title, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = $"INSERT INTO {_qualifiedTableName} (TITLE, IS_COMPLETED) VALUES (:title, :isCompleted) RETURNING ID INTO :id";
        command.Parameters.Add(new OracleParameter("title", OracleDbType.Varchar2) { Value = title });
        command.Parameters.Add(new OracleParameter("isCompleted", OracleDbType.Int16) { Value = 0 });

        var idParameter = new OracleParameter("id", OracleDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(idParameter);

        await command.ExecuteNonQueryAsync(cancellationToken);
        var id = ConvertOracleNumberToInt32(idParameter.Value);

        return new TodoItem(id, title, false);
    }

    public async Task<bool> UpdateCompletionAsync(int id, bool isCompleted, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = $"UPDATE {_qualifiedTableName} SET IS_COMPLETED = :isCompleted WHERE ID = :id";
        command.Parameters.Add(new OracleParameter("isCompleted", OracleDbType.Int16) { Value = isCompleted ? 1 : 0 });
        command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });

        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = $"DELETE FROM {_qualifiedTableName} WHERE ID = :id";
        command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });

        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected > 0;
    }

    private static TodoItem MapTodo(OracleDataReader reader)
    {
        return new TodoItem(
            ConvertOracleNumberToInt32(reader.GetValue(0)),
            reader.GetString(1),
            ConvertOracleNumberToInt32(reader.GetValue(2)) == 1);
    }

    private static int ConvertOracleNumberToInt32(object? value)
    {
        if (value is null || value is DBNull)
        {
            throw new InvalidOperationException("Expected Oracle numeric value but received null.");
        }

        if (value is OracleDecimal oracleDecimal)
        {
            return oracleDecimal.ToInt32();
        }

        if (value is decimal decimalValue)
        {
            return decimal.ToInt32(decimalValue);
        }

        return Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string BuildQualifiedTableName(OracleDatabaseOptions options)
    {
        Console.WriteLine("[DEBUG]: Building qualified table name for Oracle database. " + options.ToString());
        var tableName = ValidateIdentifier(options.TableName, nameof(options.TableName));
        if (string.IsNullOrWhiteSpace(options.Schema))
        {
            return tableName;
        }

        var schema = ValidateIdentifier(options.Schema, nameof(options.Schema));
        return $"{schema}.{tableName}";
    }

    private static string ValidateIdentifier(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Oracle database option '{propertyName}' is required.");
        }

        foreach (var character in value)
        {
            if (!(char.IsLetterOrDigit(character) || character == '_'))
            {
                throw new InvalidOperationException($"Oracle database option '{propertyName}' contains unsupported characters.");
            }
        }

        return value;
    }
}
