using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PropertyOS.Infrastructure.Data;
using PropertyOS.IntegrationTests.Authentication;
using Testcontainers.PostgreSql;

namespace PropertyOS.IntegrationTests.Database;

/// <summary>
/// Starts a throwaway PostgreSQL 17 container and builds the same database the
/// staging environment has: EF Core schema, Supabase migrations (RLS policies,
/// privilege revokes), then the least-privilege <c>propertyos_api</c> role.
/// The API under test connects ONLY as <c>propertyos_api</c>, so no test can pass
/// because a bypassing connection ignored row-level security.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private const string RuntimeRole = "propertyos_api";
    private const string RuntimePassword = "propertyos_api_test_password";

    // Supabase provides these roles; plain PostgreSQL does not. The migrations revoke
    // privileges from anon/authenticated, so stand-ins must exist before they run.
    private const string StandInRolesSql = """
        DO $$
        BEGIN
          IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'anon') THEN
            CREATE ROLE anon NOLOGIN;
          END IF;
          IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'authenticated') THEN
            CREATE ROLE authenticated NOLOGIN;
          END IF;
          IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'service_role') THEN
            CREATE ROLE service_role NOLOGIN BYPASSRLS;
          END IF;
        END $$;
        """;

#pragma warning disable CS0618 // The parameterless builder constructor is obsolete in newer Testcontainers releases.
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .Build();
#pragma warning restore CS0618

    private JwtTestWebApplicationFactory? _baseFactory;
    private WebApplicationFactory<Program>? _factory;

    internal string AdminConnectionString => _container.GetConnectionString();

    internal string RuntimeConnectionString { get; private set; } = string.Empty;

    internal string RuntimeUnpooledConnectionString { get; private set; } = string.Empty;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await ExecuteAdminScriptAsync("stand-in roles", StandInRolesSql);
        await MigrateEfSchemaAsync();

        var root = FindRepoRoot();

        var migrationFiles = Directory
            .GetFiles(Path.Combine(root, "supabase", "migrations"), "*.sql")
            .OrderBy(path => path, StringComparer.Ordinal);

        foreach (var file in migrationFiles)
        {
            await ExecuteAdminScriptFileAsync(file);
        }

        // Same order as the README: history lock-down, then the runtime role.
        await ExecuteAdminScriptFileAsync(
            Path.Combine(root, "supabase", "scripts", "secure-ef-migrations-history.sql"));
        await ExecuteAdminScriptFileAsync(
            Path.Combine(root, "supabase", "scripts", "bootstrap-api-role.sql"));

        await ExecuteAdminScriptAsync(
            "runtime role password",
            $"ALTER ROLE {RuntimeRole} PASSWORD '{RuntimePassword}'");

        var runtime = new NpgsqlConnectionStringBuilder(AdminConnectionString)
        {
            Username = RuntimeRole,
            Password = RuntimePassword,
        };
        RuntimeConnectionString = runtime.ConnectionString;

        runtime.Pooling = false;
        RuntimeUnpooledConnectionString = runtime.ConnectionString;

        var runtimeConnectionString = RuntimeConnectionString;
        _baseFactory = new JwtTestWebApplicationFactory();
        _factory = _baseFactory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:DefaultConnection", runtimeConnectionString);
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = runtimeConnectionString,
                }));
        });
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }

        if (_baseFactory is not null)
        {
            await _baseFactory.DisposeAsync();
        }

        await _container.DisposeAsync();
    }

    /// <summary>
    /// Creates an HTTP client for the real API (running as the runtime role) that
    /// sends a valid token for the given Supabase auth user and, optionally, the
    /// X-Organization-Id header.
    /// </summary>
    internal HttpClient CreateClient(Guid authUserId, Guid? organizationId = null)
    {
        var factory = _factory
            ?? throw new InvalidOperationException("The fixture has not been initialized.");

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTestWebApplicationFactory.CreateToken(subject: authUserId));

        if (organizationId is { } id)
        {
            client.DefaultRequestHeaders.Add("X-Organization-Id", id.ToString());
        }

        return client;
    }

    // ---- seeding (always as the admin role; the runtime role cannot insert these) ----

    internal async Task<Guid> SeedOrganizationAsync(string? name = null, bool isDeleted = false)
    {
        var id = Guid.NewGuid();
        var label = name ?? "Org " + id.ToString("N")[..8];

        await ExecuteAdminAsync(
            """
            INSERT INTO organizations ("Id","Name","Slug","BillingEmail","SubscriptionPlan","IsActive","IsDeleted","CreatedAt","UpdatedAt")
            VALUES (@id, @name, @slug, 'billing@example.com', 'trial', true, @deleted, now(), now())
            """,
            ("id", id),
            ("name", label),
            ("slug", "org-" + id.ToString("N")),
            ("deleted", isDeleted));

        return id;
    }

    internal async Task<(Guid UserId, Guid AuthId)> SeedUserAsync(
        Guid? authId = null,
        bool isDeleted = false)
    {
        var userId = Guid.NewGuid();
        var auth = authId ?? Guid.NewGuid();

        await ExecuteAdminAsync(
            """
            INSERT INTO users ("Id","SupabaseAuthId","Email","FirstName","LastName","Status","IsDeleted","CreatedAt","UpdatedAt")
            VALUES (@id, @auth, @email, 'Test', 'User', 0, @deleted, now(), now())
            """,
            ("id", userId),
            ("auth", auth),
            ("email", $"user-{userId:N}@example.com"),
            ("deleted", isDeleted));

        return (userId, auth);
    }

    internal async Task<Guid> SeedMembershipAsync(Guid userId, Guid organizationId)
    {
        var roleId = Guid.NewGuid();

        await ExecuteAdminAsync(
            """
            INSERT INTO roles ("Id","OrganizationId","Name","IsSystemRole","CreatedAt","UpdatedAt")
            VALUES (@role, @org, @roleName, false, now(), now())
            """,
            ("role", roleId),
            ("org", organizationId),
            ("roleName", "Member-" + roleId.ToString("N")[..8]));

        await ExecuteAdminAsync(
            """
            INSERT INTO user_organization_roles ("Id","UserId","RoleId","OrganizationId","AssignedAt")
            VALUES (gen_random_uuid(), @user, @role, @org, now())
            """,
            ("user", userId),
            ("role", roleId),
            ("org", organizationId));

        return roleId;
    }

    /// <summary>Creates an organization with one member and returns their ids.</summary>
    internal async Task<(Guid OrganizationId, Guid UserId, Guid AuthId, Guid RoleId)> SeedTenantAsync(
        string? organizationName = null)
    {
        var organizationId = await SeedOrganizationAsync(organizationName);
        var (userId, authId) = await SeedUserAsync();
        var roleId = await SeedMembershipAsync(userId, organizationId);

        return (organizationId, userId, authId, roleId);
    }

    internal async Task<int> ExecuteAdminAsync(
        string sql,
        params (string Name, object Value)[] parameters)
    {
        await using var connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        return await command.ExecuteNonQueryAsync();
    }

    internal async Task<T?> QueryAdminScalarAsync<T>(
        string sql,
        params (string Name, object Value)[] parameters)
    {
        await using var connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        var result = await command.ExecuteScalarAsync();
        return result is null or DBNull ? default : (T)result;
    }

    internal static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null
            && !File.Exists(Path.Combine(directory.FullName, "PropertyOS.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException(
                $"Could not find PropertyOS.sln above {AppContext.BaseDirectory}.");
    }

    private async Task MigrateEfSchemaAsync()
    {
        var options = new DbContextOptionsBuilder<PropertyOsDbContext>()
            .UseNpgsql(AdminConnectionString)
            .Options;

        await using var db = new PropertyOsDbContext(options);
        await db.Database.MigrateAsync();
    }

    private async Task ExecuteAdminScriptFileAsync(string path)
    {
        var sql = await File.ReadAllTextAsync(path);
        await ExecuteAdminScriptAsync(Path.GetFileName(path), sql);
    }

    private async Task ExecuteAdminScriptAsync(string description, string sql)
    {
        try
        {
            await using var connection = new NpgsqlConnection(AdminConnectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
        catch (PostgresException exception)
        {
            throw new InvalidOperationException(
                $"Test database setup failed while applying '{description}': {exception.Message}",
                exception);
        }
    }
}

/// <summary>
/// Shares one <see cref="PostgresFixture"/> (one container) across every database
/// test class and runs those classes one at a time.
/// </summary>
[CollectionDefinition(DatabaseCollection.Name)]
public sealed class DatabaseCollection : ICollectionFixture<PostgresFixture>
{
    /// <summary>The name used by <c>[Collection]</c> on database test classes.</summary>
    public const string Name = "Database";
}
