using System.Net;
using System.Text.Json;

namespace PropertyOS.IntegrationTests.Database;

/// <summary>
/// Drives the real API pipeline against the test database, with the API connected
/// as the runtime role, to prove membership checks and tenant isolation end to end.
/// </summary>
[Collection(DatabaseCollection.Name)]
public sealed class TenantIsolationApiTests
{
    private const string CurrentOrganizationPath = "/api/v1/organizations/current";

    private readonly PostgresFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantIsolationApiTests"/> class.
    /// </summary>
    /// <param name="fixture">The shared database fixture.</param>
    public TenantIsolationApiTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// A member of an organization gets that organization.
    /// </summary>
    [Fact]
    public async Task Member_GetsTheirOrganization()
    {
        var tenant = await _fixture.SeedTenantAsync("Harborview Properties");
        using var client = _fixture.CreateClient(tenant.AuthId, tenant.OrganizationId);

        var response = await client.GetAsync(CurrentOrganizationPath);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(tenant.OrganizationId, document.RootElement.GetProperty("id").GetGuid());
        Assert.Equal("Harborview Properties", document.RootElement.GetProperty("name").GetString());
        Assert.Equal("Active", document.RootElement.GetProperty("status").GetString());
    }

    /// <summary>
    /// A user who belongs to organization A cannot act for organization B.
    /// </summary>
    [Fact]
    public async Task NonMember_GetsForbiddenEnvelope()
    {
        var a = await _fixture.SeedTenantAsync();
        var b = await _fixture.SeedTenantAsync();
        using var client = _fixture.CreateClient(a.AuthId, b.OrganizationId);

        var response = await client.GetAsync(CurrentOrganizationPath);

        await ApiAssert.ErrorAsync(
            response, HttpStatusCode.Forbidden, "ORGANIZATION_ACCESS_DENIED");
    }

    /// <summary>
    /// An organization id that does not exist is indistinguishable from one the user cannot access.
    /// </summary>
    [Fact]
    public async Task UnknownOrganization_GetsForbiddenEnvelope()
    {
        var tenant = await _fixture.SeedTenantAsync();
        using var client = _fixture.CreateClient(tenant.AuthId, Guid.NewGuid());

        var response = await client.GetAsync(CurrentOrganizationPath);

        await ApiAssert.ErrorAsync(
            response, HttpStatusCode.Forbidden, "ORGANIZATION_ACCESS_DENIED");
    }

    /// <summary>
    /// A valid token for a user that has no row in the users table has no access.
    /// </summary>
    [Fact]
    public async Task UnknownUser_GetsForbiddenEnvelope()
    {
        var tenant = await _fixture.SeedTenantAsync();
        using var client = _fixture.CreateClient(Guid.NewGuid(), tenant.OrganizationId);

        var response = await client.GetAsync(CurrentOrganizationPath);

        await ApiAssert.ErrorAsync(
            response, HttpStatusCode.Forbidden, "ORGANIZATION_ACCESS_DENIED");
    }

    /// <summary>
    /// A soft-deleted user loses access even though the membership row remains.
    /// </summary>
    [Fact]
    public async Task DeletedUser_GetsForbiddenEnvelope()
    {
        var organizationId = await _fixture.SeedOrganizationAsync();
        var (userId, authId) = await _fixture.SeedUserAsync(isDeleted: true);
        await _fixture.SeedMembershipAsync(userId, organizationId);
        using var client = _fixture.CreateClient(authId, organizationId);

        var response = await client.GetAsync(CurrentOrganizationPath);

        await ApiAssert.ErrorAsync(
            response, HttpStatusCode.Forbidden, "ORGANIZATION_ACCESS_DENIED");
    }

    /// <summary>
    /// A soft-deleted organization returns 404. The middleware validates
    /// membership, while the controller excludes soft-deleted organizations
    /// from the current organization response.
    /// </summary>
    [Fact]
    public async Task DeletedOrganization_GetsNotFoundEnvelope()
    {
        var organizationId = await _fixture.SeedOrganizationAsync(isDeleted: true);
        var (userId, authId) = await _fixture.SeedUserAsync();
        await _fixture.SeedMembershipAsync(userId, organizationId);
        using var client = _fixture.CreateClient(authId, organizationId);

        var response = await client.GetAsync(CurrentOrganizationPath);

        await ApiAssert.ErrorAsync(
            response, HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND");
    }

    /// <summary>
    /// A user in two organizations gets exactly the one named in the header.
    /// </summary>
    [Fact]
    public async Task MemberOfTwoOrganizations_GetsOnlyTheRequestedOne()
    {
        var orgA = await _fixture.SeedOrganizationAsync("Org A");
        var orgB = await _fixture.SeedOrganizationAsync("Org B");
        var (userId, authId) = await _fixture.SeedUserAsync();
        await _fixture.SeedMembershipAsync(userId, orgA);
        await _fixture.SeedMembershipAsync(userId, orgB);

        using var clientA = _fixture.CreateClient(authId, orgA);
        using var clientB = _fixture.CreateClient(authId, orgB);

        var responseA = await clientA.GetAsync(CurrentOrganizationPath);
        var responseB = await clientB.GetAsync(CurrentOrganizationPath);

        Assert.Equal(HttpStatusCode.OK, responseA.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseB.StatusCode);

        using var documentA = JsonDocument.Parse(await responseA.Content.ReadAsStringAsync());
        using var documentB = JsonDocument.Parse(await responseB.Content.ReadAsStringAsync());
        Assert.Equal("Org A", documentA.RootElement.GetProperty("name").GetString());
        Assert.Equal("Org B", documentB.RootElement.GetProperty("name").GetString());
    }
}
