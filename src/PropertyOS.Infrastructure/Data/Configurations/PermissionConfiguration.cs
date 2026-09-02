using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyOS.Domain.Entities;

namespace PropertyOS.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="Permission"/> entity.
/// </summary>
public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    /// <summary>
    /// Configures the table, properties, and indexes for permissions.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Id)
            .ValueGeneratedOnAdd();

        builder.Property(permission => permission.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(permission => permission.Description)
            .HasMaxLength(500);

        builder.Property(permission => permission.Domain)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(permission => permission.CreatedAt)
            .IsRequired();

        builder.Property(permission => permission.UpdatedAt)
            .IsRequired();

        builder.HasIndex(permission => permission.Code)
            .IsUnique();

        builder.HasIndex(permission => permission.Domain);
    }
}