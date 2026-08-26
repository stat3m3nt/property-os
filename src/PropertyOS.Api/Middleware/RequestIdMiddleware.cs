namespace PropertyOS.Api.Middleware;

/// <summary>
/// Adds a unique request identifier to each HTTP request for correlation
/// across API responses and application logs.
/// </summary>
public sealed class RequestIdMiddleware
{
    private const string RequestIdHeader = "X-Request-Id";

    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestIdMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    public RequestIdMiddleware(RequestDelegate next)
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
        var requestId = Guid.NewGuid().ToString();

        context.Items[RequestIdHeader] = requestId;
        context.Response.Headers[RequestIdHeader] = requestId;

        await _next(context);
    }
}