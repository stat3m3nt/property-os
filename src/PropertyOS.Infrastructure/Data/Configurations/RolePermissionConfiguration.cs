using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyOS.Domain.Entities;

namespace PropertyOS.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="RolePermission"/> entity.
/// </summary>
public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    /// <summary>
    /// Configures the table, properties, indexes, and relationships for role-permission assignments.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rolePermission => rolePermission.Id);

        builder.Property(rolePermission => rolePermission.Id)
            .ValueGeneratedOnAdd();

        builder.Property(rolePermission => rolePermission.RoleId)
            .IsRequired();

        builder.Property(rolePermission => rolePermission.PermissionId)
            .IsRequired();

        builder.Property(rolePermission => rolePermission.CreatedAt)
            .IsRequired();

        builder.Property(rolePermission => rolePermission.CreatedBy);

        builder.HasIndex(rolePermission => new
        {
            rolePermission.RoleId,
            rolePermission.PermissionId
        })
        .IsUnique();

        builder.HasOne(rolePermission => rolePermission.Role)
            .WithMany(role => role.RolePermissions)
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rolePermission => rolePermission.Permission)
            .WithMany(permission => permission.RolePermissions)
            .HasForeignKey(rolePermission => rolePermission.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}