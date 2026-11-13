namespace PropertyOS.Api.Errors;

/// <summary>
/// Machine-readable error codes returned in the standard error envelope
/// (see docs/07-api-design.md section 6.5).
/// </summary>
public static class ErrorCodes
{
    /// <summary>Field-level or request-level validation failure.</summary>
    public const string ValidationFailed = "VALIDATION_FAILED";

    /// <summary>Missing, invalid or expired access token.</summary>
    public const string Unauthorized = "UNAUTHORIZED";

    /// <summary>The caller lacks a required permission.</summary>
    public const string InsufficientPermission = "INSUFFICIENT_PERMISSION";

    /// <summary>The caller is not a member of the requested organization.</summary>
    public const string OrganizationAccessDenied = "ORGANIZATION_ACCESS_DENIED";

    /// <summary>The resource is missing, deleted or outside the caller's tenant scope.</summary>
    public const string ResourceNotFound = "RESOURCE_NOT_FOUND";

    /// <summary>The HTTP method is not supported for the resource.</summary>
    public const string MethodNotAllowed = "METHOD_NOT_ALLOWED";

    /// <summary>Generic state conflict.</summary>
    public const string Conflict = "CONFLICT";

    /// <summary>The request content type is not supported.</summary>
    public const string UnsupportedMediaType = "UNSUPPORTED_MEDIA_TYPE";

    /// <summary>Too many requests.</summary>
    public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";

    /// <summary>Unhandled server error.</summary>
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";

    /// <summary>Fallback for statuses without a more specific code.</summary>
    public const string RequestFailed = "REQUEST_FAILED";
}
