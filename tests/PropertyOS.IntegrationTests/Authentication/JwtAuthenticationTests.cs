using System.Net;
using System.Net.Http.Headers;

namespace PropertyOS.IntegrationTests.Authentication;

/// <summary>
/// Verifies the JWT bearer validation pipeline configured in
/// <c>Program.cs</c> against valid, expired, malformed, tampered, missing,
/// and wrong-issuer tokens, using <c>/api/v1/auth/me</c> as the target
/// <c>[Authorize]</c> endpoint.
/// </summary>
public sealed class JwtAuthenticationTests : IClassFixture<JwtTestWebApplicationFactory>
{
    private const string ProtectedEndpoint = "/api/v1/auth/me";

    private readonly JwtTestWebApplicationFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtAuthenticationTests"/> class.
    /// </summary>
    /// <param name="factory">
    /// The test application factory used to create the API test server.
    /// </param>
    public JwtAuthenticationTests(JwtTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Verifies that a valid JWT is accepted by the protected endpoint.
    /// </summary>
    [Fact]
    public async Task ValidToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var token = JwtTestWebApplicationFactory.CreateToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(ProtectedEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Verifies that a request without a JWT is rejected by the protected endpoint.
    /// </summary>
    [Fact]
    public async Task MissingToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(ProtectedEndpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Verifies that an expired JWT is rejected by the protected endpoint.
    /// </summary>
    [Fact]
    public async Task ExpiredToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var token = JwtTestWebApplicationFactory.CreateToken(expired: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(ProtectedEndpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Verifies that a malformed JWT is rejected by the protected endpoint.
    /// </summary>
    [Fact]
    public async Task MalformedToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-real-jwt");

        var response = await client.GetAsync(ProtectedEndpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Verifies that a JWT with an invalid signature is rejected by the protected endpoint.
    /// </summary>
    [Fact]
    public async Task TamperedSignature_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var token = JwtTestWebApplicationFactory.CreateToken();

        // Flip the first character of the signature segment so the payload
        // and header remain well-formed but the signature no longer verifies.
        var segments = token.Split('.');
        var signature = segments[2];
        var flipped = (signature[0] == 'A' ? 'B' : 'A') + signature[1..];
        var tamperedToken = $"{segments[0]}.{segments[1]}.{flipped}";

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tamperedToken);

        var response = await client.GetAsync(ProtectedEndpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Verifies that a JWT with an incorrect issuer is rejected by the protected endpoint.
    /// </summary>
    [Fact]
    public async Task WrongIssuer_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var token = JwtTestWebApplicationFactory.CreateToken(
            issuer: "https://not-the-configured-issuer.example.com/auth/v1");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(ProtectedEndpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Verifies that the health endpoint remains accessible without authentication.
    /// </summary>
    [Fact]
    public async Task ValidToken_HealthEndpointStillWorksAnonymously()
    {
        // Sanity check that adding JWT auth to the pipeline didn't
        // accidentally lock down the intentionally anonymous health check.
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}