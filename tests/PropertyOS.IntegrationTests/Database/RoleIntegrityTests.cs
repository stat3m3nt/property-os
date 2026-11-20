
using Npgsql;

namespace PropertyOS.IntegrationTests.Database;

/// <summary>
/// Verifies PostgreSQL enforces global and organization-owned role integrity.
/// These tests use the privileged test connection to isolate database
/// constraints from runtime-role row-level security restrictions.
/// </summary>
[Collection(DatabaseCollection.Name)]
public sealed class RoleIntegrityTests
{
    private readonly PostgresFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleIntegrityTests"/> class.
    /// </summary>
    /// <param name="fixture">The shared PostgreSQL test fixture.</param>
    public RoleIntegrityTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that a global role with valid classification can be inserted.
    /// </summary>
    [Fact]
    public async Task GlobalRole_WithValidClassification_CanBeInserted()
    {
        await InsertRoleAsync(
            organizationId: null,
            name: "Global-" + Guid.NewGuid().ToString("N"),
            isSystemRole: true);
    }

    /// <summary>
    /// Verifies that an organization-owned role with valid classification can be inserted.
    /// </summary>
    [Fact]
    public async Task OrganizationRole_WithValidClassification_CanBeInserted()
    {
        var organizationId = await _fixture.SeedOrganizationAsync();

        await InsertRoleAsync(
            organizationId,
            "Custom-" + Guid.NewGuid().ToString("N"),
            isSystemRole: false);
    }

    /// <summary>
    /// Verifies that PostgreSQL rejects a global role marked as non-system.
    /// </summary>
    [Fact]
    public async Task GlobalRole_WithIsSystemRoleFalse_IsRejected()
    {
        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertRoleAsync(
                organizationId: null,
                name: "Invalid-Global-" + Guid.NewGuid().ToString("N"),
                isSystemRole: false));

        Assert.Equal("23514", exception.SqlState);
        Assert.Equal("CK_roles_SystemRoleOrganization", exception.ConstraintName);
    }

    /// <summary>
    /// Verifies that PostgreSQL rejects an organization-owned role marked as system.
    /// </summary>
    [Fact]
    public async Task OrganizationRole_WithIsSystemRoleTrue_IsRejected()
    {
        var organizationId = await _fixture.SeedOrganizationAsync();

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertRoleAsync(
                organizationId,
                "Invalid-Org-" + Guid.NewGuid().ToString("N"),
                isSystemRole: true));

        Assert.Equal("23514", exception.SqlState);
        Assert.Equal("CK_roles_SystemRoleOrganization", exception.ConstraintName);
    }

    /// <summary>
    /// Verifies that PostgreSQL rejects duplicate names among global roles.
    /// </summary>
    [Fact]
    public async Task GlobalRoles_WithDuplicateNames_AreRejected()
    {
        var name = "Duplicate-Global-" + Guid.NewGuid().ToString("N");

        await InsertRoleAsync(null, name, isSystemRole: true);

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertRoleAsync(null, name, isSystemRole: true));

        Assert.Equal("23505", exception.SqlState);
        Assert.Equal("IX_roles_Global_Name", exception.ConstraintName);
    }

    /// <summary>
    /// Inserts a role using the privileged test connection.
    /// </summary>
    /// <param name="organizationId">The owning organization, or null for a global role.</param>
    /// <param name="name">The role name.</param>
    /// <param name="isSystemRole">Whether the role is a global system role.</param>
    /// <returns>The number of rows inserted.</returns>
    private Task<int> InsertRoleAsync(
        Guid? organizationId,
        string name,
        bool isSystemRole)
    {
        return _fixture.ExecuteAdminAsync(
            """
            INSERT INTO roles
                ("Id", "OrganizationId", "Name", "IsSystemRole", "CreatedAt", "UpdatedAt")
            VALUES
                (@id, @organizationId, @name, @isSystemRole, now(), now())
            """,
            ("id", Guid.NewGuid()),
            ("organizationId", (object?)organizationId ?? DBNull.Value),
            ("name", name),
            ("isSystemRole", isSystemRole));
    }
}
