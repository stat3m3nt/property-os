using Microsoft.AspNetCore.Mvc.Testing;

namespace PropertyOS.IntegrationTests;

/// <summary>
/// Verifies that the PropertyOS API can start and process HTTP requests
/// through the ASP.NET Core integration test pipeline.
/// </summary>
public sealed class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes the integration test client.
    /// </summary>
    /// <param name="factory">The ASP.NET Core application factory.</param>
    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Verifies that the health endpoint responds successfully.
    /// </summary>
    [Fact]
    public async Task HealthEndpoint_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/v1/health");

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected successful response but received {(int)response.StatusCode}.");
    }
}