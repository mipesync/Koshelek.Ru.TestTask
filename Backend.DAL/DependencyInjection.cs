using Backend.DAL.Managers;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Backend.DAL;

/// <summary>
/// Implementing data access layer dependencies
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adding Data Access Layer dependencies
    /// </summary>
    public static void AddDataAccessLayer(this IServiceCollection serviceCollection, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) throw new NpgsqlException("The connection string must not be null");
        
        serviceCollection.AddSingleton<MessageManager>(provider => new MessageManager(connectionString, provider));
        serviceCollection.AddSingleton<DatabaseInitializer>(provider => new DatabaseInitializer(connectionString, provider));
    }
}