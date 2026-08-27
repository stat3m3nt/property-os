namespace PropertyOS.Domain.Entities;

/// <summary>
/// Represents a role that defines a set of permissions within PropertyOS.
/// </summary>
public sealed class Role
{
    /// <summary>
    /// Gets or sets the unique identifier of the role.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the organization that owns the role.
    /// A null value indicates a system-defined global role.
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the role.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the role is a system-defined role.
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the role was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the role was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the role.
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the role.
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the organization associated with the role.
    /// </summary>
    public Organization? Organization { get; set; }

    /// <summary>
    /// Gets or sets the users assigned to this role within their organizations.
    /// </summary>
    public ICollection<UserOrganizationRole> UserOrganizationRoles { get; set; }
        = new List<UserOrganizationRole>();

    /// <summary>
    /// Gets or sets the permissions assigned to this role.
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}