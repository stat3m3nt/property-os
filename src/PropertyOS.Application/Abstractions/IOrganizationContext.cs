namespace PropertyOS.Application.Abstractions;

/// <summary>
/// Provides the active organization context for the current request.
/// </summary>
public interface IOrganizationContext
{
    /// <summary>
    /// Gets the active organization ID, if one has been resolved.
    /// </summary>
    Guid? OrganizationId { get; }

    /// <summary>
    /// Gets whether an organization has been resolved.
    /// </summary>
    bool IsResolved { get; }

    /// <summary>
    /// Sets the active organization ID for the current request.
    /// </summary>
    /// <param name="organizationId">The validated organization ID.</param>
    void SetOrganizationId(Guid organizationId);
}
