using Execora.Core.Enums;

namespace Execora.Core.Entities;

/// <summary>
/// Immutable record of security-relevant events.
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant context (nullable for system events)
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Name of entity (User, Tenant, Invitation, etc.)
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// ID of affected entity
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Type of action performed
    /// </summary>
    public AuditAction Action { get; set; }

    /// <summary>
    /// Detailed action type
    /// </summary>
    public string? ActionType { get; set; }

    /// <summary>
    /// Previous values (JSON)
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// New values (JSON)
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// User who made the change
    /// </summary>
    public Guid? ChangedBy { get; set; }

    /// <summary>
    /// User, System, API
    /// </summary>
    public string? ChangedByType { get; set; }

    /// <summary>
    /// When the change occurred
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// IP address of requester
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Client user agent string
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Optional project context
    /// </summary>
    public Guid? ProjectId { get; set; }
}
