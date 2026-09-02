using PropertyOS.Application.Abstractions;

namespace PropertyOS.Infrastructure.Authentication;

/// <summary>
/// Represents the organization context for the current request.
/// </summary>
public sealed class OrganizationContext : IOrganizationContext
{
    /// <inheritdoc />
    public Guid? OrganizationId { get; private set; }

    /// <inheritdoc />
    public bool IsResolved => OrganizationId.HasValue;

    /// <summary>
    /// Sets the organization identifier for the current request.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    public void SetOrganizationId(Guid organizationId)
    {
        OrganizationId = organizationId;
    }
}