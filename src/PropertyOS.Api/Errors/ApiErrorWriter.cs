using System.Text.Json;
using System.Text.Json.Serialization;

namespace PropertyOS.Api.Errors;

/// <summary>
/// Describes one field-level problem inside an error response.
/// </summary>
/// <param name="Field">The field, header or permission the problem refers to.</param>
/// <param name="Issue">A human-readable description of the problem.</param>
public sealed record ErrorDetail(string Field, string Issue);

/// <summary>
/// The body of the standard error envelope.
/// </summary>
/// <param name="Code">A stable machine-readable error code.</param>
/// <param name="Message">A human-readable message.</param>
/// <param name="RequestId">The correlation id, also returned in the X-Request-Id header.</param>
/// <param name="Timestamp">The UTC time the error was produced.</param>
/// <param name="Details">Optional field-level details.</param>
public sealed record ErrorBody(
    string Code,
    string Message,
    string RequestId,
    DateTime Timestamp,
    IReadOnlyList<ErrorDetail>? Details = null);

/// <summary>
/// The standard error response: <c>{ "error": { ... } }</c>.
/// </summary>
/// <param name="Error">The error body.</param>
public sealed record ErrorEnvelope(ErrorBody Error);

/// <summary>
/// Writes the standard error envelope (docs/07-api-design.md section 6) to a response.
/// Every non-2xx response produced by the API goes through this one writer.
/// </summary>
public static class ApiErrorWriter
{
    private const string RequestIdKey = "X-Request-Id";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// Writes an error envelope with the given status, code and message.
    /// Does nothing if the response has already started.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable message.</param>
    /// <param name="details">Optional field-level details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task WriteAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IReadOnlyList<ErrorDetail>? details = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        var requestId = context.Items[RequestIdKey]?.ToString()
            ?? Guid.NewGuid().ToString();

        var envelope = new ErrorEnvelope(
            new ErrorBody(code, message, requestId, DateTime.UtcNow, details));

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(envelope, JsonOptions));
    }
}
