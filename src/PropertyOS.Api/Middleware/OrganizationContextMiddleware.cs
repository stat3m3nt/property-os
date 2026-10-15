using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PropertyOS.Application.Abstractions;
using PropertyOS.Domain.Entities;
using PropertyOS.Infrastructure.Data;

namespace PropertyOS.Api.Middleware;

/// <summary>
/// Resolves and validates the active organization and establishes transaction-scoped RLS context.
/// </summary>
public sealed class OrganizationContextMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes the middleware.
    /// </summary>
    /// <param name="next">The next request delegate.</param>
    public OrganizationContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Resolves the organization, validates membership, and runs the request in a database transaction.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="db">The request-scoped database context.</param>
    /// <param name="organizationContext">The current organization context.</param>
    public async Task InvokeAsync(
        HttpContext context,
        PropertyOsDbContext db,
        IOrganizationContext organizationContext)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // These endpoints do not require a selected tenant.
        if ((string.Equals(path, "/health", StringComparison.OrdinalIgnoreCase) || string.Equals(path, "/api/v1/health", StringComparison.OrdinalIgnoreCase)) ||
            string.Equals(path, "/api/v1/auth/me", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Let authentication/authorization handle unauthenticated requests.
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        if (!Guid.TryParse(
                context.Request.Headers["X-Organization-Id"].ToString(),
                out var organizationId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "A valid X-Organization-Id header is required."
            });
            return;
        }

        // Supabase's verified JWT subject identifies the external auth user.
        var subject = context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(subject, out var supabaseAuthId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // The tenant setting and all request queries must share this transaction.
        await using var transaction =
            await db.Database.BeginTransactionAsync(context.RequestAborted);

        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT set_config('app.current_org_id', {organizationId.ToString()}, true)",
                context.RequestAborted);

            // Validate membership using the verified JWT identity, never a
            // user ID supplied by the caller. RLS is active for this query.
            var isMember = await db.Set<User>().AnyAsync(
                user => user.SupabaseAuthId == supabaseAuthId
                    && !user.IsDeleted
                    && user.UserOrganizationRoles.Any(
                        membership => membership.OrganizationId == organizationId),
                context.RequestAborted);

            if (!isMember)
            {
                await transaction.RollbackAsync(context.RequestAborted);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "You do not have access to this organization."
                });
                return;
            }

            organizationContext.SetOrganizationId(organizationId);

            await _next(context);

            if (context.Response.StatusCode >= 400)
            {
                await transaction.RollbackAsync(context.RequestAborted);
            }
            else
            {
                await transaction.CommitAsync(context.RequestAborted);
            }
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}