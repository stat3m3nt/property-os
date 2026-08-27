using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyOS.Domain.Entities;

namespace PropertyOS.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="UserOrganizationRole"/> entity.
/// </summary>
public sealed class UserOrganizationRoleConfiguration
    : IEntityTypeConfiguration<UserOrganizationRole>
{
    /// <summary>
    /// Configures the table, properties, indexes, and relationships
    /// for organization role assignments.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<UserOrganizationRole> builder)
    {
        builder.ToTable("user_organization_roles");

        builder.HasKey(userOrganizationRole => userOrganizationRole.Id);

        builder.Property(userOrganizationRole => userOrganizationRole.Id)
            .ValueGeneratedOnAdd();

        builder.Property(userOrganizationRole => userOrganizationRole.UserId)
            .IsRequired();

        builder.Property(userOrganizationRole => userOrganizationRole.RoleId)
            .IsRequired();

        builder.Property(userOrganizationRole => userOrganizationRole.OrganizationId)
            .IsRequired();

        builder.Property(userOrganizationRole => userOrganizationRole.AssignedAt)
            .IsRequired();

        builder.Property(userOrganizationRole => userOrganizationRole.CreatedBy);

        builder.HasIndex(userOrganizationRole => new
        {
            userOrganizationRole.UserId,
            userOrganizationRole.OrganizationId,
            userOrganizationRole.RoleId
        })
        .IsUnique();

        builder.HasIndex(userOrganizationRole => new
        {
            userOrganizationRole.UserId,
            userOrganizationRole.OrganizationId
        });

        builder.HasOne(userOrganizationRole => userOrganizationRole.User)
            .WithMany(user => user.UserOrganizationRoles)
            .HasForeignKey(userOrganizationRole => userOrganizationRole.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(userOrganizationRole => userOrganizationRole.Role)
            .WithMany(role => role.UserOrganizationRoles)
            .HasForeignKey(userOrganizationRole => userOrganizationRole.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(userOrganizationRole => userOrganizationRole.Organization)
            .WithMany(organization => organization.UserOrganizationRoles)
            .HasForeignKey(userOrganizationRole => userOrganizationRole.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}