
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyOS.Application.Abstractions;
using PropertyOS.Infrastructure.Data;
using PropertyOS.Domain.Entities;

namespace PropertyOS.Api.Controllers;

/// <summary>
/// Represents the response returned by the current organization endpoint.
/// </summary>
public sealed record OrganizationResponse(Guid Id, string Name, string Status);

/// <summary>
/// Provides endpoints for retrieving information about the current organization.
/// </summary>
[ApiController]
[Route("api/v1/organizations")]
[Authorize]
public sealed class OrganizationsController : ControllerBase
{
    private readonly PropertyOsDbContext _db;
    private readonly IOrganizationContext _organizationContext;
    /// <summary>
    /// Initializes the controller with the database and current organization context.
    /// </summary>
    /// <param name="db">The application database context.</param>
    /// <param name="organizationContext">The validated organization context for the request.</param>
    public OrganizationsController(
        PropertyOsDbContext db,
        IOrganizationContext organizationContext)
    {
        _db = db;
        _organizationContext = organizationContext;
    }

    /// <summary>
    /// Returns the currently selected organization.
    /// </summary>
    [HttpGet("current")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationResponse>> GetCurrent(
        CancellationToken cancellationToken)
    {
        if (!_organizationContext.OrganizationId.HasValue)
        {
            return Unauthorized();
        }

        var organizationId = _organizationContext.OrganizationId.Value;

        var organization = await _db.Set<Organization>()
            .AsNoTracking()
            .Where(organization =>
                organization.Id == organizationId &&
                !organization.IsDeleted)
            .Select(organization => new OrganizationResponse(
                organization.Id,
                organization.Name,
                organization.IsActive ? "Active" : "Inactive"))
            .SingleOrDefaultAsync(cancellationToken);

        if (organization is null)
        {
            return NotFound();
        }

        return Ok(organization);
    }
}