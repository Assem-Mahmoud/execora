namespace Execora.Core.Enums;

/// <summary>
/// Status of an invitation to join a tenant
/// </summary>
public enum InvitationStatus
{
    /// <summary>
    /// Invitation has been sent but not yet accepted
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Invitation has been accepted and user has joined
    /// </summary>
    Accepted = 2,

    /// <summary>
    /// Invitation has expired (past expiration date)
    /// </summary>
    Expired = 3,

    /// <summary>
    /// Invitation has been cancelled by the sender
    /// </summary>
    Cancelled = 4
}
