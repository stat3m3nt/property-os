namespace PropertyOS.Domain.Entities;

/// <summary>
/// Represents a tenant organization within the PropertyOS platform.
/// </summary>
public sealed class Organization
{
    /// <summary>
    /// Gets or sets the unique identifier of the organization.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the organization's display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique URL-friendly identifier for the organization.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the organization's legal name.
    /// </summary>
    public string? LegalName { get; set; }

    /// <summary>
    /// Gets or sets the email address used for organization billing.
    /// </summary>
    public string BillingEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Stripe customer identifier associated with the organization.
    /// </summary>
    public string? StripeCustomerId { get; set; }

    /// <summary>
    /// Gets or sets the subscription plan assigned to the organization.
    /// </summary>
    public string SubscriptionPlan { get; set; } = "trial";

    /// <summary>
    /// Gets or sets a value indicating whether the organization is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the organization has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the organization was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the organization was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the organization was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the organization.
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the organization.
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the user and role memberships associated with the organization.
    /// </summary>
    public ICollection<UserOrganizationRole> UserOrganizationRoles { get; set; }
        = new List<UserOrganizationRole>();
    
    /// <summary>
    /// Gets or sets the roles associated with the organization.
    /// </summary>
    public ICollection<Role> Roles { get; set; }
        = new List<Role>();
}