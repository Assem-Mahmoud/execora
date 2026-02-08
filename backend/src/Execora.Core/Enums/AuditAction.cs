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
    /// User's password was reset
    /// </summary>
    PasswordReset = 9,

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
    TenantSwitched = 14
}
