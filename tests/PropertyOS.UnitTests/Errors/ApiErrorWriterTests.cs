using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using PropertyOS.Api.Errors;

namespace PropertyOS.UnitTests.Errors;

/// <summary>
/// Verifies the shared error writer and the helpers that feed it, so every
/// non-2xx response follows docs/07-api-design.md section 6.
/// </summary>
public sealed class ApiErrorWriterTests
{
    /// <summary>
    /// Verifies the status, content type and required envelope fields.
    /// </summary>
    [Fact]
    public async Task WriteAsync_WritesStandardEnvelope()
    {
        var context = CreateContext();
        context.Items["X-Request-Id"] = "req-123";

        await ApiErrorWriter.WriteAsync(
            context, StatusCodes.Status403Forbidden, "SOME_CODE", "Some message.");

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var error = await ReadErrorAsync(context);
        Assert.Equal("SOME_CODE", error.GetProperty("code").GetString());
        Assert.Equal("Some message.", error.GetProperty("message").GetString());
        Assert.Equal("req-123", error.GetProperty("requestId").GetString());
        Assert.True(error.TryGetProperty("timestamp", out _));
    }

    /// <summary>
    /// Verifies that the details property is left out when there are none.
    /// </summary>
    [Fact]
    public async Task WriteAsync_OmitsDetails_WhenNoneProvided()
    {
        var context = CreateContext();

        await ApiErrorWriter.WriteAsync(context, 400, "SOME_CODE", "Some message.");

        var error = await ReadErrorAsync(context);
        Assert.False(error.TryGetProperty("details", out _));
    }

    /// <summary>
    /// Verifies that field-level details are serialized.
    /// </summary>
    [Fact]
    public async Task WriteAsync_IncludesDetails_WhenProvided()
    {
        var context = CreateContext();

        await ApiErrorWriter.WriteAsync(
            context,
            400,
            ErrorCodes.ValidationFailed,
            "Invalid.",
            [new ErrorDetail("X-Organization-Id", "Must be a valid organization GUID.")]);

        var error = await ReadErrorAsync(context);
        var detail = error.GetProperty("details")[0];
        Assert.Equal("X-Organization-Id", detail.GetProperty("field").GetString());
        Assert.Equal("Must be a valid organization GUID.", detail.GetProperty("issue").GetString());
    }

    /// <summary>
    /// Verifies the status-to-code mapping against the documented registry.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="expectedCode">The expected error code.</param>
    [Theory]
    [InlineData(400, "VALIDATION_FAILED")]
    [InlineData(401, "UNAUTHORIZED")]
    [InlineData(403, "INSUFFICIENT_PERMISSION")]
    [InlineData(404, "RESOURCE_NOT_FOUND")]
    [InlineData(405, "METHOD_NOT_ALLOWED")]
    [InlineData(409, "CONFLICT")]
    [InlineData(415, "UNSUPPORTED_MEDIA_TYPE")]
    [InlineData(429, "RATE_LIMIT_EXCEEDED")]
    [InlineData(500, "INTERNAL_SERVER_ERROR")]
    [InlineData(503, "INTERNAL_SERVER_ERROR")]
    [InlineData(418, "REQUEST_FAILED")]
    public void StatusCodeErrors_MapsStatusToCode(int statusCode, string expectedCode)
    {
        var (code, message) = StatusCodeErrors.For(statusCode);

        Assert.Equal(expectedCode, code);
        Assert.False(string.IsNullOrWhiteSpace(message));
    }

    /// <summary>
    /// Verifies that invalid model state becomes a 400 VALIDATION_FAILED envelope
    /// with one detail per model error.
    /// </summary>
    [Fact]
    public async Task ModelStateErrors_ToResult_ProducesValidationEnvelope()
    {
        var httpContext = CreateContext();
        var actionContext = new ActionContext(
            httpContext, new RouteData(), new ActionDescriptor());
        actionContext.ModelState.AddModelError("unitId", "unitId is required");

        var result = ModelStateErrors.ToResult(actionContext);
        await result.ExecuteResultAsync(actionContext);

        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);

        var error = await ReadErrorAsync(httpContext);
        Assert.Equal("VALIDATION_FAILED", error.GetProperty("code").GetString());

        var detail = error.GetProperty("details")[0];
        Assert.Equal("unitId", detail.GetProperty("field").GetString());
        Assert.Equal("unitId is required", detail.GetProperty("issue").GetString());
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<JsonElement> ReadErrorAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        using var document = JsonDocument.Parse(body);
        return document.RootElement.GetProperty("error").Clone();
    }
}
