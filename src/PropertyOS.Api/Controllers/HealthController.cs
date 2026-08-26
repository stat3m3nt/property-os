using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace PropertyOS.Api.Controllers;

/// <summary>
/// Provides endpoints for monitoring the operational health of the PropertyOS API.
/// </summary>
/// <remarks>
/// Health endpoints are used by monitoring systems, deployment pipelines,
/// load balancers, and other infrastructure components to verify that the API
/// is running and capable of processing incoming requests.
/// </remarks>
[ApiController]
[Route("api/v1/health")]
[Authorize]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Returns the current operational health status of the PropertyOS API.
    /// </summary>
    /// <remarks>
    /// A successful response indicates that the API is running and able to
    /// process requests. Additional dependency checks, such as database
    /// connectivity and external service availability, may be added in future
    /// iterations.
    /// </remarks>
    /// <returns>
    /// A <c>200 OK</c> response containing the API's current health status,
    /// service identifier, and the current UTC timestamp.
    /// </returns>
    /// <response code="200">
    /// The API is operational and successfully processed the health check request.
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "PropertyOS.Api",
            timestamp = DateTime.UtcNow
        });
    }
}