using Microsoft.AspNetCore.Mvc;

namespace PropertyOS.Api.Controllers;

/// <summary>
/// Provides endpoints for retrieving information about the current organization.
/// </summary>
/// <remarks>
/// Organization endpoints are tenant-aware. In Sprint 1 these endpoints return
/// stub data while the organization and multi-tenant infrastructure is being
/// established. Future sprints will resolve the current organization from the
/// authenticated user's context.
/// </remarks>
[ApiController]
[Route("api/v1/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    /// <summary>
    /// Returns the currently selected organization.
    /// </summary>
    /// <remarks>
    /// This endpoint currently returns placeholder data and will be implemented
    /// once organization persistence and tenant resolution are introduced.
    /// </remarks>
    /// <returns>
    /// A <c>200 OK</c> response containing placeholder organization information.
    /// </returns>
    /// <response code="200">
    /// The current organization was successfully retrieved.
    /// </response>
    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCurrent()
    {
        return Ok(new
        {
            id = Guid.Empty,
            name = "Default Organization",
            status = "Stub"
        });
    }
}