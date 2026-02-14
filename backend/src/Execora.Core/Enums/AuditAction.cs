namespace Execora.Core.Enums;

/// <summary>
/// Types of auditable actions in the system
/// </summary>
public enum AuditAction
{
    /// <summary>
    /// Entity was created
    /// </summary>
    Created = 1,

    /// <summary>
    /// Entity was updated
    /// </summary>
    Updated = 2,

    /// <summary>
    /// Entity was deleted
    /// </summary>
    Deleted = 3,

    /// <summary>
    /// Entity state was changed (status transition)
    /// </summary>
    StateChanged = 4,

    /// <summary>
    /// Entity was viewed/accessed
    /// </summary>
    Viewed = 5,

    /// <summary>
    /// User logged in
    /// </summary>
    LoggedIn = 6,

    /// <summary>
    /// User logged out
    /// </summary>
    LoggedOut = 7,

    /// <summary>
    /// User changed their password
    /// </summary>
    PasswordChanged = 8,

    /// <summary>
    /// Password change failed
    /// </summary>
    PasswordChangeFailed = 9,

    /// <summary>
    /// User's password was reset
    /// </summary>
    PasswordReset = 10,

    /// <summary>
    /// User's email was verified
    /// </summary>
    EmailVerified = 10,

    /// <summary>
    /// Invitation was sent
    /// </summary>
    InvitationSent = 11,

    /// <summary>
    /// Invitation was accepted
    /// </summary>
    InvitationAccepted = 12,

    /// <summary>
    /// User's role was changed
    /// </summary>
    RoleChanged = 13,

    /// <summary>
    /// User switched active tenant
    /// </summary>
    TenantSwitched = 14,

    /// <summary>
    /// Password reset was requested
    /// </summary>
    PasswordResetRequested = 16,

    /// <summary>
    /// Password reset failed
    /// </summary>
    PasswordResetFailed = 17,

    /// <summary>
    /// General security event
    /// </summary>
    SecurityEvent = 18,

    /// <summary>
    /// Error occurred
    /// </summary>
    Error = 19
}
