using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyOS.Domain.Entities;

namespace PropertyOS.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="Role"/> entity.
/// </summary>
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <summary>
    /// Configures the table, properties, indexes, and relationships for roles.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Id)
            .ValueGeneratedOnAdd();

        builder.Property(role => role.OrganizationId);

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(role => role.Description)
            .HasMaxLength(500);

        builder.Property(role => role.IsSystemRole)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(role => role.CreatedAt)
            .IsRequired();

        builder.Property(role => role.UpdatedAt)
            .IsRequired();

        builder.Property(role => role.CreatedBy);

        builder.Property(role => role.UpdatedBy);

        builder.HasIndex(role => new
        {
            role.OrganizationId,
            role.Name
        })
        .IsUnique();

        builder.HasOne(role => role.Organization)
            .WithMany(organization => organization.Roles)
            .HasForeignKey(role => role.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}