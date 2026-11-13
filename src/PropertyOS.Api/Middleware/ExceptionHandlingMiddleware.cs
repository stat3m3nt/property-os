using PropertyOS.Api.Errors;

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

            // Make sure the id in the response is the one that was logged.
            context.Items[RequestIdKey] = requestId;

            _logger.LogError(
                exception,
                "Unhandled exception processing request {RequestId}",
                requestId);

            var (code, message) = StatusCodeErrors.For(
                StatusCodes.Status500InternalServerError);

            await ApiErrorWriter.WriteAsync(
                context,
                StatusCodes.Status500InternalServerError,
                code,
                message);
        }
    }
}
