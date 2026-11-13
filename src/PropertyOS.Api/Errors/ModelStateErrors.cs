using Microsoft.AspNetCore.Mvc;

namespace PropertyOS.Api.Errors;

/// <summary>
/// An action result that writes the standard error envelope.
/// </summary>
/// <param name="statusCode">The HTTP status code.</param>
/// <param name="code">The machine-readable error code.</param>
/// <param name="message">The human-readable message.</param>
/// <param name="details">Optional field-level details.</param>
public sealed class ApiErrorResult(
    int statusCode,
    string code,
    string message,
    IReadOnlyList<ErrorDetail>? details = null) : IActionResult
{
    /// <inheritdoc />
    public Task ExecuteResultAsync(ActionContext context) =>
        ApiErrorWriter.WriteAsync(context.HttpContext, statusCode, code, message, details);
}

/// <summary>
/// Converts automatic <c>[ApiController]</c> model-validation failures into
/// the standard error envelope instead of ASP.NET Core's ProblemDetails.
/// </summary>
public static class ModelStateErrors
{
    /// <summary>
    /// Builds a 400 VALIDATION_FAILED result from the invalid model state.
    /// </summary>
    /// <param name="actionContext">The action context holding the model state.</param>
    /// <returns>The error result.</returns>
    public static IActionResult ToResult(ActionContext actionContext)
    {
        var details = actionContext.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(entry => entry.Value!.Errors.Select(error => new ErrorDetail(
                entry.Key,
                string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "The value is invalid."
                    : error.ErrorMessage)))
            .ToList();

        return new ApiErrorResult(
            StatusCodes.Status400BadRequest,
            ErrorCodes.ValidationFailed,
            "The request contains one or more validation errors.",
            details);
    }
}
