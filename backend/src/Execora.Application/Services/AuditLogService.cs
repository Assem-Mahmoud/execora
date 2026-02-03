using System.Diagnostics;
using System.Text.Json;
using Execora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Execora.Application.Services;

/// <summary>
/// In-memory audit log service for Phase 2
/// Note: In production, this should write to an immutable audit log table or dedicated audit service
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly ILogger<AuditLogService> _logger;
    private readonly ExecoraDbContext? _context;

    public AuditLogService(ILogger<AuditLogService> logger, ExecoraDbContext? context = null)
    {
        _logger = logger;
        _context = context;
    }

    public Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        // Log to structured logging for now
        // In production, this would write to the AuditLog table
        _logger.LogInformation(
            "Audit: {Action} {EntityName}/{EntityId} by {ChangedBy} (Tenant: {TenantId})",
            entry.Action,
            entry.EntityName,
            entry.EntityId,
            entry.ChangedBy,
            entry.TenantId);

        // TODO: Implement persistent audit log storage when AuditLog entity is created
        // Example:
        // if (_context != null)
        // {
        //     var auditLog = new AuditLog
        //     {
        //         Id = Guid.NewGuid(),
        //         TenantId = entry.TenantId,
        //         EntityName = entry.EntityName,
        //         EntityId = entry.EntityId,
        //         Action = entry.Action,
        //         // ... map other properties
        //     };
        //     _context.AuditLogs.Add(auditLog);
        //     await _context.SaveChangesAsync(cancellationToken);
        // }

        return Task.CompletedTask;
    }

    public Task LogCreateAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        object newValues,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        return LogAsync(new AuditLogEntry
        {
            TenantId = tenantId,
            EntityName = entityName,
            EntityId = entityId,
            Action = AuditAction.Created,
            NewValues = JsonSerializer.Serialize(newValues),
            ChangedBy = changedBy,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ProjectId = projectId
        }, cancellationToken);
    }

    public Task LogUpdateAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        object? oldValues,
        object newValues,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        return LogAsync(new AuditLogEntry
        {
            TenantId = tenantId,
            EntityName = entityName,
            EntityId = entityId,
            Action = AuditAction.Updated,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = JsonSerializer.Serialize(newValues),
            ChangedBy = changedBy,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ProjectId = projectId
        }, cancellationToken);
    }

    public Task LogDeleteAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        object? oldValues,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        return LogAsync(new AuditLogEntry
        {
            TenantId = tenantId,
            EntityName = entityName,
            EntityId = entityId,
            Action = AuditAction.Deleted,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            ChangedBy = changedBy,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ProjectId = projectId
        }, cancellationToken);
    }

    public Task LogStateChangeAsync(
        Guid tenantId,
        string entityName,
        Guid entityId,
        string? oldState,
        string newState,
        Guid? changedBy,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        return LogAsync(new AuditLogEntry
        {
            TenantId = tenantId,
            EntityName = entityName,
            EntityId = entityId,
            Action = AuditAction.StateChanged,
            ActionType = $"{oldState} -> {newState}",
            ChangedBy = changedBy,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ProjectId = projectId
        }, cancellationToken);
    }
}
