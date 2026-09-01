using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace PropertyOS.IntegrationTests.Authentication;

/// <summary>
/// Runs the real PropertyOS API pipeline (as configured in <c>Program.cs</c>),
/// but replaces the production JWT bearer configuration's Supabase-hosted
/// signing-key discovery (<c>Authority</c> / JWKS, which requires live network
/// access) with a local symmetric key.
/// </summary>
/// <remarks>
/// This does not change how <c>Program.cs</c> is configured for real traffic —
/// it only overrides <see cref="JwtBearerOptions"/> for this test host via
/// <c>PostConfigure</c>, so the validation logic itself (issuer, audience,
/// lifetime, signature checks) can be exercised deterministically and offline.
/// </remarks>
public sealed class JwtTestWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Gets the issuer value used by the test JWT tokens.
    /// </summary>
    public const string Issuer = "https://test-issuer.propertyos.local/auth/v1";

    /// <summary>
    /// Gets the audience value used by the test JWT tokens.
    /// </summary>
    public const string Audience = "authenticated";

    private static readonly SymmetricSecurityKey SigningKey =
        new(Encoding.UTF8.GetBytes("test-signing-key-only-used-in-ci-never-a-real-secret-32b+"));

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = null;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,

                    ValidateAudience = true,
                    ValidAudience = Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SigningKey,

                    ClockSkew = TimeSpan.FromSeconds(60),
                };
            });
        });
    }

    /// <summary>
    /// Mints a JWT signed with the test symmetric key, matching the shape
    /// of a real Supabase-issued token closely enough to exercise the
    /// validation pipeline (this does not attempt to replicate Supabase's
    /// custom claims — that lands with the Sprint 2 claims hook).
    /// </summary>
    public static string CreateToken(bool expired = false, string issuer = Issuer)
    {
        var handler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, "test-user@example.com"),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: Audience,
            claims: claims,
            notBefore: expired ? DateTime.UtcNow.AddHours(-2) : DateTime.UtcNow.AddMinutes(-1),
            expires: expired ? DateTime.UtcNow.AddHours(-1) : DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256));

        return handler.WriteToken(token);
    }
}