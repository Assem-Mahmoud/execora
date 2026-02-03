using Execora.Core.Enums;

namespace Execora.Core.Entities;

/// <summary>
/// Junction table for user-tenant relationships with role information.
/// </summary>
public class TenantUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Reference to the tenant
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Reference to the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Role assigned to this user within the tenant
    /// </summary>
    public TenantRole Role { get; set; }

    /// <summary>
    /// Additional permissions (rare, stored as JSON)
    /// </summary>
    public string? Permissions { get; set; }

    /// <summary>
    /// Who invited this user
    /// </summary>
    public Guid? InvitedBy { get; set; }

    /// <summary>
    /// Invitation timestamp
    /// </summary>
    public DateTime? InvitedAt { get; set; }

    /// <summary>
    /// Acceptance timestamp
    /// </summary>
    public DateTime? JoinedAt { get; set; }

    /// <summary>
    /// Membership active flag
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}
