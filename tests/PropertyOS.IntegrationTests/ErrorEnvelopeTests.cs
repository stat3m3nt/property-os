using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using PropertyOS.IntegrationTests.Authentication;

namespace PropertyOS.IntegrationTests;

/// <summary>
/// Verifies that error responses produced by the real pipeline use the
/// standard error envelope (docs/07-api-design.md section 6). Cases that
/// need a database (403 for a non-member, 404 for a missing organization)
/// belong in the Testcontainers suite.
/// </summary>
public sealed class ErrorEnvelopeTests : IClassFixture<JwtTestWebApplicationFactory>
{
    private readonly JwtTestWebApplicationFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorEnvelopeTests"/> class.
    /// </summary>
    /// <param name="factory">The test application factory.</param>
    public ErrorEnvelopeTests(JwtTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// A request with no token gets a 401 envelope, not an empty body.
    /// </summary>
    [Fact]
    public async Task MissingToken_Returns401Envelope()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        await AssertEnvelopeAsync(response, HttpStatusCode.Unauthorized, "UNAUTHORIZED");
    }

    /// <summary>
    /// An expired token gets a 401 envelope.
    /// </summary>
    [Fact]
    public async Task ExpiredToken_Returns401Envelope()
    {
        using var client = CreateAuthenticatedClient(
            JwtTestWebApplicationFactory.CreateToken(expired: true));

        var response = await client.GetAsync("/api/v1/auth/me");

        await AssertEnvelopeAsync(response, HttpStatusCode.Unauthorized, "UNAUTHORIZED");
    }

    /// <summary>
    /// A malformed token gets a 401 envelope.
    /// </summary>
    [Fact]
    public async Task MalformedToken_Returns401Envelope()
    {
        using var client = CreateAuthenticatedClient("not-a-real-jwt");

        var response = await client.GetAsync("/api/v1/auth/me");

        await AssertEnvelopeAsync(response, HttpStatusCode.Unauthorized, "UNAUTHORIZED");
    }

    /// <summary>
    /// A missing organization header gets a 400 envelope naming the header.
    /// </summary>
    [Fact]
    public async Task MissingOrganizationHeader_Returns400Envelope()
    {
        using var client = CreateAuthenticatedClient(
            JwtTestWebApplicationFactory.CreateToken());

        var response = await client.GetAsync("/api/v1/organizations/current");

        var error = await AssertEnvelopeAsync(
            response, HttpStatusCode.BadRequest, "VALIDATION_FAILED");
        Assert.Equal(
            "X-Organization-Id",
            error.GetProperty("details")[0].GetProperty("field").GetString());
    }

    /// <summary>
    /// A malformed organization header gets a 400 envelope.
    /// </summary>
    [Fact]
    public async Task InvalidOrganizationHeader_Returns400Envelope()
    {
        using var client = CreateAuthenticatedClient(
            JwtTestWebApplicationFactory.CreateToken());
        client.DefaultRequestHeaders.Add("X-Organization-Id", "not-a-guid");

        var response = await client.GetAsync("/api/v1/organizations/current");

        await AssertEnvelopeAsync(
            response, HttpStatusCode.BadRequest, "VALIDATION_FAILED");
    }

    /// <summary>
    /// An unknown route gets a 404 envelope.
    /// </summary>
    [Fact]
    public async Task UnknownRoute_Returns404Envelope()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/this-route-does-not-exist");

        await AssertEnvelopeAsync(
            response, HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND");
    }

    private HttpClient CreateAuthenticatedClient(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static async Task<JsonElement> AssertEnvelopeAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedCode)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var error = document.RootElement.GetProperty("error").Clone();

        Assert.Equal(expectedCode, error.GetProperty("code").GetString());
        Assert.False(string.IsNullOrWhiteSpace(error.GetProperty("message").GetString()));
        Assert.Equal(
            response.Headers.GetValues("X-Request-Id").Single(),
            error.GetProperty("requestId").GetString());
        Assert.True(error.TryGetProperty("timestamp", out _));

        return error;
    }
}
