using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PropertyOS.Api.Controllers;

/// <summary>
/// Represents the response returned by the authentication endpoint.
/// </summary>
public sealed record ClaimDto(string Type, string Value);

/// <summary>
/// Represents the response returned by the authentication endpoint.
/// </summary>
/// <param name="IsAuthenticated">Indicates whether the user is authenticated.</param>
/// <param name="Claims">A collection of claims associated with the authenticated user.</param>
public sealed record AuthMeResponse(bool IsAuthenticated, IEnumerable<ClaimDto> Claims);


/// <summary>
/// Provides endpoints for testing and inspecting authenticated user access.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    /// <summary>
    /// Returns information about the currently authenticated user.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(AuthMeResponse), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser()
    {
        var claims = User.Claims
            .Select(c => new ClaimDto(c.Type, c.Value))
            .ToList();

        return Ok(new AuthMeResponse(
            User.Identity?.IsAuthenticated ?? false,
            claims));
    }
}