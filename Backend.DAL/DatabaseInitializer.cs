using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Backend.DAL;

/// <summary>
/// Database init
/// </summary>
public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(string connectionString, IServiceProvider serviceProvider)
    {
        _connectionString = connectionString;
        _logger = serviceProvider.GetService<ILogger<DatabaseInitializer>>()!;
    }

    public void EnsureDatabaseCreated()
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.Database;
            builder.Database = "postgres";

            using (var connection = new NpgsqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'",
                           connection))
                {
                    var exists = command.ExecuteScalar() != null;

                    if (!exists)
                    {
                        _logger.LogInformation($"Database '{databaseName}' does not exist. Creating...");

                        using var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
                        createCommand.ExecuteNonQuery();
                        _logger.LogInformation($"Database '{databaseName}' created successfully");
                    }
                    else
                    {
                        _logger.LogInformation($"Database '{databaseName}' already exists");
                    }
                }
            }

            builder.Database = databaseName;
            using (var connection = new NpgsqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(
                           "SELECT EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'messages')", connection))
                {
                    var tableExists = (bool)(command.ExecuteScalar() ?? false);

                    if (!tableExists)
                    {
                        _logger.LogInformation("Table 'Messages' does not exist. Creating...");

                        using var createTableCommand = new NpgsqlCommand(
                            "CREATE TABLE Messages (" +
                            "Id SERIAL PRIMARY KEY, " +
                            "Content VARCHAR(128) NOT NULL, " +
                            "Timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW(), " +
                            "OrderId INT NOT NULL)", connection);
                        createTableCommand.ExecuteNonQuery();
                        _logger.LogInformation("Table 'Messages' created successfully");
                    }
                    else
                    {
                        _logger.LogInformation("Table 'Messages' already exists");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database");
        }
    }
}
