using Backend.Application.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application;

/// <summary>
/// Implementing business logic layer dependencies
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adding Business Logic Layer dependencies
    /// </summary>
    public static void AddBusinessLogicLayer(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<MessageRepository>();
    }
}