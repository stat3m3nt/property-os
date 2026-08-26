namespace PropertyOS.Application.Abstractions;

/// <summary>
/// Provides access to the organization context associated with the current request.
/// </summary>
public interface IOrganizationContext
{
    /// <summary>
    /// Gets the current organization identifier.
    /// </summary>
    Guid? OrganizationId { get; }

    /// <summary>
    /// Gets a value indicating whether an organization has been resolved for the current request.
    /// </summary>
    bool IsResolved { get; }
}