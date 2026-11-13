namespace PropertyOS.Api.Errors;

/// <summary>
/// Maps an HTTP status code to the error code and message used when a
/// response leaves the pipeline with an error status and no body
/// (for example a JWT 401, a 403 from authorization, or an unknown route).
/// </summary>
public static class StatusCodeErrors
{
    /// <summary>
    /// Returns the error code and message for a status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>The error code and message.</returns>
    public static (string Code, string Message) For(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest =>
            (ErrorCodes.ValidationFailed, "The request is invalid."),
        StatusCodes.Status401Unauthorized =>
            (ErrorCodes.Unauthorized, "Authentication is required, or the access token is invalid or expired."),
        StatusCodes.Status403Forbidden =>
            (ErrorCodes.InsufficientPermission, "Your role does not have permission to perform this action."),
        StatusCodes.Status404NotFound =>
            (ErrorCodes.ResourceNotFound, "The requested resource was not found."),
        StatusCodes.Status405MethodNotAllowed =>
            (ErrorCodes.MethodNotAllowed, "The HTTP method is not allowed for this resource."),
        StatusCodes.Status409Conflict =>
            (ErrorCodes.Conflict, "The request conflicts with the current state of the resource."),
        StatusCodes.Status415UnsupportedMediaType =>
            (ErrorCodes.UnsupportedMediaType, "The request content type is not supported."),
        StatusCodes.Status429TooManyRequests =>
            (ErrorCodes.RateLimitExceeded, "Too many requests. Please retry later."),
        >= 500 =>
            (ErrorCodes.InternalServerError, "An unexpected error occurred. Please contact support with the requestId below."),
        _ =>
            (ErrorCodes.RequestFailed, "The request could not be completed."),
    };
}
