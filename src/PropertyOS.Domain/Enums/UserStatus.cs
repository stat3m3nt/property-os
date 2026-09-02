namespace PropertyOS.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a PropertyOS user account.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// Indicates that the user account is active.
    /// </summary>
    Active,

    /// <summary>
    /// Indicates that the user account is inactive.
    /// </summary>
    Inactive,

    /// <summary>
    /// Indicates that the user account has been suspended.
    /// </summary>
    Suspended,

    /// <summary>
    /// Indicates that the user account is awaiting verification.
    /// </summary>
    PendingVerification
}