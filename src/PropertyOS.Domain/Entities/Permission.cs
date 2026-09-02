namespace PropertyOS.Domain.Entities;

/// <summary>
/// Represents a permission that can be assigned to a role.
/// </summary>
public sealed class Permission
{
    /// <summary>
    /// Gets or sets the unique identifier of the permission.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique permission code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the permission.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the functional domain to which the permission belongs.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the permission was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the permission was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the roles that have been assigned this permission.
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}