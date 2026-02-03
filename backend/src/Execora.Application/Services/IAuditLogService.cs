using Execora.Core.Enums;

namespace Execora.Application.Services;

/// <summary>
/// Audit log types for tracking different kinds of changes
/// </summary>
public enum AuditAction
{
    Created,
    Updated,
    Deleted,
    StateChanged,
    Viewed,
    Login,
    Logout,
    Exported
}

/// <summary>
/// Audit log entry
/// </summary>
public record AuditLogEntry
{
    public Guid? TenantId { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public Guid? EntityId { get; init; }
    public AuditAction Action { get; init; }
    public string? ActionType { get; init; }
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public Guid? ChangedBy { get; init; }
    public string? ChangedByType { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public Guid? ProjectId { get; init; }
}

/// <summary>
/// Service for creating immutable audit trail entries
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Logs an audit entry
    /// </summary>
    Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a create action
    /// </summary>
    Task LogCreateAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        object newValues,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an update action
    /// </summary>
    Task LogUpdateAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        object? oldValues,
        object newValues,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a delete action
    /// </summary>
    Task LogDeleteAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        object? oldValues,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a state change action
    /// </summary>
    Task LogStateChangeAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        string? oldState,
        string newState,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default);
}
