namespace PropertyOS.Domain.Entities;

/// <summary>
/// Represents a user's membership and assigned role within an organization.
/// </summary>
public sealed class UserOrganizationRole
{
    /// <summary>
    /// Gets or sets the unique identifier of the organization role assignment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the role.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the organization.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the role was assigned.
    /// </summary>
    public DateTimeOffset AssignedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who assigned the role.
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the associated user.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated role.
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated organization.
    /// </summary>
    public Organization Organization { get; set; } = null!;
}