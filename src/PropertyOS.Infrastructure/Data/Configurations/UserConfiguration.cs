using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyOS.Domain.Entities;

namespace PropertyOS.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="User"/> entity.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the table, properties, indexes, and relationships for users.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedOnAdd();

        builder.Property(user => user.SupabaseAuthId)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(user => user.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.Phone)
            .HasMaxLength(30);

        builder.Property(user => user.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(user => user.LastLoginAt);

        builder.Property(user => user.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(user => user.DeletedAt);

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt)
            .IsRequired();

        builder.Property(user => user.CreatedBy);

        builder.Property(user => user.UpdatedBy);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.HasIndex(user => user.SupabaseAuthId)
            .IsUnique();
    }
}