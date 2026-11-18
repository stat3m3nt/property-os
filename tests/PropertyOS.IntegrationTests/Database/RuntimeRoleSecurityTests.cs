using Npgsql;

namespace PropertyOS.IntegrationTests.Database;

/// <summary>
/// Talks to PostgreSQL directly as the runtime role (<c>propertyos_api</c>),
/// with no API code in between, to prove row-level security and privileges
/// hold on their own.
/// </summary>
[Collection(DatabaseCollection.Name)]
public sealed class RuntimeRoleSecurityTests
{
    private readonly PostgresFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeRoleSecurityTests"/> class.
    /// </summary>
    /// <param name="fixture">The shared database fixture.</param>
    public RuntimeRoleSecurityTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Runs verify-api-role.sql against the test database; every check must pass.
    /// </summary>
    [Fact]
    public async Task VerifyScript_AllChecksPass()
    {
        var path = Path.Combine(
            PostgresFixture.FindRepoRoot(), "supabase", "scripts", "verify-api-role.sql");

        await using var connection = new NpgsqlConnection(_fixture.AdminConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(await File.ReadAllTextAsync(path), connection);
        await using var reader = await command.ExecuteReaderAsync();

        var failed = new List<string>();
        var total = 0;
        while (await reader.ReadAsync())
        {
            total++;
            if (!reader.GetBoolean(1))
            {
                failed.Add(reader.GetString(0));
            }
        }

        Assert.True(total > 0, "The verify script returned no rows.");
        Assert.True(failed.Count == 0, "Failed checks: " + string.Join("; ", failed));
    }

    /// <summary>
    /// With organization A active, only A's rows are visible in every tenant table.
    /// </summary>
    [Fact]
    public async Task OrganizationContext_ShowsOnlyThatOrganizationsRows()
    {
        var a = await _fixture.SeedTenantAsync();
        var b = await _fixture.SeedTenantAsync();

        await using var connection = await OpenRuntimeAsync();
        await using var transaction = await BeginInOrganizationAsync(connection, a.OrganizationId);

        var organizations = await QueryIdsAsync(connection, transaction, """SELECT "Id" FROM organizations""");
        Assert.Contains(a.OrganizationId, organizations);
        Assert.DoesNotContain(b.OrganizationId, organizations);

        var users = await QueryIdsAsync(connection, transaction, """SELECT "Id" FROM users""");
        Assert.Contains(a.UserId, users);
        Assert.DoesNotContain(b.UserId, users);

        var roles = await QueryIdsAsync(connection, transaction, """SELECT "Id" FROM roles""");
        Assert.Contains(a.RoleId, roles);
        Assert.DoesNotContain(b.RoleId, roles);

        var memberships = await QueryIdsAsync(
            connection, transaction, """SELECT "OrganizationId" FROM user_organization_roles""");
        Assert.Contains(a.OrganizationId, memberships);
        Assert.DoesNotContain(b.OrganizationId, memberships);
    }

    /// <summary>
    /// A connection that never set an organization sees no tenant rows and does not error.
    /// </summary>
    [Fact]
    public async Task NoOrganizationContext_ShowsNoTenantRows()
    {
        await _fixture.SeedTenantAsync();

        await using var connection = await OpenRuntimeAsync();

        Assert.Equal(0L, await CountAsync(connection, null, "SELECT count(*) FROM organizations"));
        Assert.Equal(0L, await CountAsync(connection, null, "SELECT count(*) FROM users"));
        Assert.Equal(
            0L,
            await CountAsync(
                connection, null, """SELECT count(*) FROM roles WHERE "OrganizationId" IS NOT NULL"""));
    }

    /// <summary>
    /// Regression test for the empty-string setting: after a request transaction ends,
    /// a reused connection must return no rows, not raise an invalid-uuid error.
    /// </summary>
    [Fact]
    public async Task EmptySettingAfterTransaction_ReturnsNoRowsWithoutError()
    {
        var tenant = await _fixture.SeedTenantAsync();

        await using var connection = await OpenRuntimeAsync();

        await using (var transaction = await BeginInOrganizationAsync(connection, tenant.OrganizationId))
        {
            Assert.Equal(1L, await CountAsync(connection, transaction, "SELECT count(*) FROM organizations"));
            await transaction.CommitAsync();
        }

        Assert.Equal(0L, await CountAsync(connection, null, "SELECT count(*) FROM organizations"));
    }

    /// <summary>
    /// The runtime role cannot create organizations; that goes through a privileged path.
    /// </summary>
    [Fact]
    public async Task RuntimeRole_CannotInsertOrganization()
    {
        await using var connection = await OpenRuntimeAsync();

        await using var command = new NpgsqlCommand(
            """
            INSERT INTO organizations ("Id","Name","Slug","BillingEmail","SubscriptionPlan","IsActive","IsDeleted","CreatedAt","UpdatedAt")
            VALUES (gen_random_uuid(), 'Evil', 'evil', 'x@example.com', 'trial', true, false, now(), now())
            """,
            connection);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    /// <summary>
    /// With organization A active, a role cannot be written into organization B.
    /// </summary>
    [Fact]
    public async Task RuntimeRole_CannotWriteRoleIntoAnotherOrganization()
    {
        var a = await _fixture.SeedTenantAsync();
        var b = await _fixture.SeedTenantAsync();

        await using var connection = await OpenRuntimeAsync();
        await using var transaction = await BeginInOrganizationAsync(connection, a.OrganizationId);

        await using var command = new NpgsqlCommand(
            """
            INSERT INTO roles ("Id","OrganizationId","Name","IsSystemRole","CreatedAt","UpdatedAt")
            VALUES (gen_random_uuid(), @org, 'Smuggled', false, now(), now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("org", b.OrganizationId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    /// <summary>
    /// With organization A active, updating organization B affects no rows.
    /// </summary>
    [Fact]
    public async Task RuntimeRole_CannotUpdateAnotherOrganization()
    {
        var a = await _fixture.SeedTenantAsync();
        var b = await _fixture.SeedTenantAsync("Untouched");

        await using var connection = await OpenRuntimeAsync();
        await using var transaction = await BeginInOrganizationAsync(connection, a.OrganizationId);

        await using var command = new NpgsqlCommand(
            """UPDATE organizations SET "Name" = 'Hijacked' WHERE "Id" = @id""",
            connection,
            transaction);
        command.Parameters.AddWithValue("id", b.OrganizationId);

        Assert.Equal(0, await command.ExecuteNonQueryAsync());

        var name = await _fixture.QueryAdminScalarAsync<string>(
            """SELECT "Name" FROM organizations WHERE "Id" = @id""",
            ("id", b.OrganizationId));
        Assert.Equal("Untouched", name);
    }

    /// <summary>
    /// The runtime role has no DDL rights in the public schema.
    /// </summary>
    [Fact]
    public async Task RuntimeRole_CannotCreateObjects()
    {
        await using var connection = await OpenRuntimeAsync();

        await using var command = new NpgsqlCommand("CREATE TABLE public.should_not_exist (id int)", connection);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    /// <summary>
    /// The runtime role cannot read EF Core's migration history.
    /// </summary>
    [Fact]
    public async Task RuntimeRole_CannotReadMigrationHistory()
    {
        await using var connection = await OpenRuntimeAsync();

        await using var command = new NpgsqlCommand(
            """SELECT count(*) FROM public."__EFMigrationsHistory" """,
            connection);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteScalarAsync());
        Assert.Equal("42501", exception.SqlState);
    }

    private async Task<NpgsqlConnection> OpenRuntimeAsync()
    {
        var connection = new NpgsqlConnection(_fixture.RuntimeUnpooledConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    private static async Task<NpgsqlTransaction> BeginInOrganizationAsync(
        NpgsqlConnection connection,
        Guid organizationId)
    {
        var transaction = await connection.BeginTransactionAsync();

        await using var command = new NpgsqlCommand(
            "SELECT set_config('app.current_org_id', @org, true)",
            connection,
            transaction);
        command.Parameters.AddWithValue("org", organizationId.ToString());
        await command.ExecuteScalarAsync();

        return transaction;
    }

    private static async Task<List<Guid>> QueryIdsAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync();

        var ids = new List<Guid>();
        while (await reader.ReadAsync())
        {
            ids.Add(reader.GetGuid(0));
        }

        return ids;
    }

    private static async Task<long> CountAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        return (long)(await command.ExecuteScalarAsync())!;
    }
}
