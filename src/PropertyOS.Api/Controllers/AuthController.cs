using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PropertyOS.Api.Controllers;

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
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated,
            UserId = User.FindFirst("sub")?.Value,
            Email = User.FindFirst("email")?.Value
        });
    }
}