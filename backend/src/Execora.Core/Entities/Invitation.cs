using Execora.Core.Enums;

namespace Execora.Core.Entities;

/// <summary>
/// Represents a pending invitation for a user to join a tenant.
/// </summary>
public class Invitation : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant user is invited to
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Email address of invited user
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Role to assign upon acceptance
    /// </summary>
    public TenantRole Role { get; set; }

    /// <summary>
    /// Secure invitation token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration (7 days)
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Current status of the invitation
    /// </summary>
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

    /// <summary>
    /// User who sent invitation
    /// </summary>
    public Guid InvitedBy { get; set; }

    /// <summary>
    /// When invitation was sent
    /// </summary>
    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When invitation was accepted
    /// </summary>
    public DateTime? AcceptedAt { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
}
