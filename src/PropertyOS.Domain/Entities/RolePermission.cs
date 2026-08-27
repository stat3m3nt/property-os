namespace PropertyOS.Domain.Entities;

/// <summary>
/// Represents the assignment of a permission to a role.
/// </summary>
public sealed class RolePermission
{
    /// <summary>
    /// Gets or sets the unique identifier of the role-permission assignment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the role.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the permission.
    /// </summary>
    public Guid PermissionId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the permission was assigned to the role.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who assigned the permission.
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the associated role.
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated permission.
    /// </summary>
    public Permission Permission { get; set; } = null!;
}