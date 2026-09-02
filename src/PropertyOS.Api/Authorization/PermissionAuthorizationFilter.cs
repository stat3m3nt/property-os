using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PropertyOS.Application.Authorization;

namespace PropertyOS.Api.Authorization;

/// <summary>
/// Enforces permissions declared through <see cref="RequirePermissionAttribute"/>.
/// </summary>
public sealed class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    /// <summary>
    /// Authorizes the current request based on permission claims in the authenticated user.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var attributes = context.ActionDescriptor.EndpointMetadata
            .OfType<RequirePermissionAttribute>()
            .ToArray();

        // Endpoints without [RequirePermission] do not require permission checks.
        if (attributes.Length == 0)
        {
            return Task.CompletedTask;
        }

        // An endpoint requiring a permission must have an authenticated user.
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        var permissions = context.HttpContext.User
            .FindAll("permission")
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var hasRequiredPermissions = attributes.All(
            attribute => permissions.Contains(attribute.Permission));

        if (!hasRequiredPermissions)
        {
            context.Result = new ForbidResult();
        }

        return Task.CompletedTask;
    }
}