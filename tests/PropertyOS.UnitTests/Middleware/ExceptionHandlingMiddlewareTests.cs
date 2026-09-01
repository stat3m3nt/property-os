using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using PropertyOS.Api.Middleware;

namespace PropertyOS.UnitTests.Middleware;

/// <summary>
/// Verifies that <see cref="ExceptionHandlingMiddleware"/> produces the
/// standardized error envelope documented in docs/07-api-design.md §6.4
/// for any unhandled exception.
/// </summary>
public sealed class ExceptionHandlingMiddlewareTests
{
    /// <summary>
    /// Verifies that an unhandled exception is converted into a 500 response
    /// carrying the documented error envelope shape.
    /// </summary>
    [Fact]
    public async Task UnhandledException_Returns500WithStandardErrorEnvelope()
    {
        var middleware = new ExceptionHandlingMiddleware(
            next: _ => throw new InvalidOperationException("boom"),
            logger: NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var error = await ReadErrorEnvelopeAsync(context);

        Assert.Equal("INTERNAL_SERVER_ERROR", error.GetProperty("code").GetString());
        Assert.False(string.IsNullOrWhiteSpace(error.GetProperty("message").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(error.GetProperty("requestId").GetString()));
        Assert.True(error.TryGetProperty("timestamp", out _));
    }

    /// <summary>
    /// Verifies that the error envelope reuses the request ID already
    /// established by <see cref="RequestIdMiddleware"/> earlier in the
    /// pipeline, rather than generating an unrelated second one.
    /// </summary>
    [Fact]
    public async Task UnhandledException_UsesExistingRequestId_WhenSetByUpstreamMiddleware()
    {
        const string expectedRequestId = "test-request-id-123";

        var middleware = new ExceptionHandlingMiddleware(
            next: _ => throw new InvalidOperationException("boom"),
            logger: NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Items["X-Request-Id"] = expectedRequestId;

        await middleware.InvokeAsync(context);

        var error = await ReadErrorEnvelopeAsync(context);

        Assert.Equal(expectedRequestId, error.GetProperty("requestId").GetString());
    }

    private static async Task<JsonElement> ReadErrorEnvelopeAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        using var document = JsonDocument.Parse(body);
        return document.RootElement.GetProperty("error").Clone();
    }
}
