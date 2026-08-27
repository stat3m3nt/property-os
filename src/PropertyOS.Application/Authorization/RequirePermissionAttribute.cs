namespace PropertyOS.Application.Authorization;

/// <summary>
/// Specifies the permission required to access an endpoint.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true)]
public sealed class RequirePermissionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission code required to access the endpoint.</param>
    public RequirePermissionAttribute(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentException(
                "Permission cannot be null or whitespace.",
                nameof(permission));
        }

        Permission = permission;
    }

    /// <summary>
    /// Gets the permission code required to access the endpoint.
    /// </summary>
    public string Permission { get; }
}