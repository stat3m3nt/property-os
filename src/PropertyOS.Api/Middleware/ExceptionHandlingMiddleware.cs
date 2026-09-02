using System.Text.Json;

namespace PropertyOS.Api.Middleware;

/// <summary>
/// Handles unhandled exceptions and converts them into the standardized
/// PropertyOS error response format.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private const string RequestIdKey = "X-Request-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logger">The application logger.</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes the current HTTP request and handles unhandled exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var requestId = context.Items[RequestIdKey]?.ToString()
                ?? Guid.NewGuid().ToString();

            _logger.LogError(
                exception,
                "Unhandled exception processing request {RequestId}",
                requestId);

            await WriteErrorResponseAsync(context, requestId);
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        string requestId)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                code = "INTERNAL_SERVER_ERROR",
                message = "An unexpected error occurred. Please contact support with the requestId below.",
                requestId,
                timestamp = DateTime.UtcNow
            }
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}