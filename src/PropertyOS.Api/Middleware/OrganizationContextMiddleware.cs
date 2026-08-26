namespace PropertyOS.Api.Middleware;

/// <summary>
/// Resolves the organization context for the current request.
/// </summary>
public sealed class OrganizationContextMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationContextMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    public OrganizationContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes the current HTTP request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Organization resolution will be implemented in Sprint 2
        // once organization entities and tenant membership exist.

        await _next(context);
    }
}