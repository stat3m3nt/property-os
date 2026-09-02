using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyOS.Domain.Entities;

namespace PropertyOS.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="Organization"/> entity.
/// </summary>
public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    /// <summary>
    /// Configures the table, properties, indexes, and relationships for organizations.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");

        builder.HasKey(organization => organization.Id);

        builder.Property(organization => organization.Id)
            .ValueGeneratedOnAdd();

        builder.Property(organization => organization.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(organization => organization.Slug)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(organization => organization.LegalName)
            .HasMaxLength(255);

        builder.Property(organization => organization.BillingEmail)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(organization => organization.StripeCustomerId)
            .HasMaxLength(255);

        builder.Property(organization => organization.SubscriptionPlan)
            .HasMaxLength(50)
            .HasDefaultValue("trial")
            .IsRequired();

        builder.Property(organization => organization.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(organization => organization.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(organization => organization.DeletedAt);

        builder.Property(organization => organization.CreatedAt)
            .IsRequired();

        builder.Property(organization => organization.UpdatedAt)
            .IsRequired();

        builder.Property(organization => organization.CreatedBy);

        builder.Property(organization => organization.UpdatedBy);

        builder.HasIndex(organization => organization.Slug)
            .IsUnique();

        builder.HasIndex(organization => organization.IsActive);
    }
}