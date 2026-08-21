using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyOS.Infrastructure.Data;

namespace PropertyOS.Infrastructure.DependencyInjection;

/// <summary>
/// Provides dependency injection configuration for the infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services with the dependency injection container.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PropertyOsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}