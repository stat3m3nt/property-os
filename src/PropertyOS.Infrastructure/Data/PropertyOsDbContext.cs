using Microsoft.EntityFrameworkCore;

namespace PropertyOS.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the PropertyOS application.
/// </summary>
public class PropertyOsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyOsDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context configuration options.</param>
    public PropertyOsDbContext(DbContextOptions<PropertyOsDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the database model and its entity relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the database model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PropertyOsDbContext).Assembly);
    }
}