using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using PropertyOS.Api.Authorization;
using PropertyOS.Application.Authorization;
using System.Security.Claims;

namespace PropertyOS.UnitTests.Authorization;

/// <summary>
/// Contains unit tests for the permission authorization filter.
/// </summary>
public sealed class PermissionAuthorizationFilterTests
{
    /// <summary>
/// Verifies that an unauthenticated user receives an unauthorized result
/// when accessing an endpoint that requires a permission.
/// </summary>
[Fact]
public async Task UnauthenticatedUser_WithRequiredPermission_ReturnsUnauthorized()
    {
        var filter = new PermissionAuthorizationFilter();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };

        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = new List<object>
            {
                new RequirePermissionAttribute("properties.read")
            }
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            actionDescriptor);

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        await filter.OnAuthorizationAsync(context);

        Assert.IsType<UnauthorizedResult>(context.Result);
    }

    /// <summary>
    /// Verifies that an authenticated user is allowed when no permission
    /// requirement is present on the endpoint.
    /// </summary>
    [Fact]
    public async Task AuthenticatedUser_WithNoRequiredPermission_AllowsRequest()
    {
        var filter = new PermissionAuthorizationFilter();

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user")
            },
            authenticationType: "Test");

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = new List<object>()
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            actionDescriptor);

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        await filter.OnAuthorizationAsync(context);

        Assert.Null(context.Result);
    }

    /// <summary>
    /// Verifies that an authenticated user with the required permission
    /// is allowed to access the endpoint.
    /// </summary>
    [Fact]
    public async Task AuthenticatedUser_WithRequiredPermission_AllowsRequest()
    {
        var filter = new PermissionAuthorizationFilter();

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim("permission", "properties.read")
            },
            authenticationType: "Test");

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = new List<object>
            {
                new RequirePermissionAttribute("properties.read")
            }
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            actionDescriptor);

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        await filter.OnAuthorizationAsync(context);

        Assert.Null(context.Result);
    }

    /// <summary>
    /// Verifies that an authenticated user without the required permission
    /// receives a forbidden result.
    /// </summary>
    [Fact]
    public async Task AuthenticatedUser_WithMissingRequiredPermission_ReturnsForbidden()
    {
        var filter = new PermissionAuthorizationFilter();

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim("permission", "properties.write")
            },
            authenticationType: "Test");

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = new List<object>
            {
                new RequirePermissionAttribute("properties.read")
            }
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            actionDescriptor);

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        await filter.OnAuthorizationAsync(context);

        Assert.IsType<ForbidResult>(context.Result);
    }

    /// <summary>
    /// Verifies that permission matching is case-insensitive.
    /// </summary>
    [Fact]
    public async Task PermissionMatching_IsCaseInsensitive()
    {
        var filter = new PermissionAuthorizationFilter();

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim("permission", "PROPERTIES.READ")
            },
            authenticationType: "Test");

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = new List<object>
            {
                new RequirePermissionAttribute("properties.read")
            }
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            actionDescriptor);

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        await filter.OnAuthorizationAsync(context);

        Assert.Null(context.Result);
    }
}