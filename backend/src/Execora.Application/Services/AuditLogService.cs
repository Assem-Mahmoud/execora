using Execora.Core.Enums;
using System.Diagnostics;
using System.Text.Json;
using Execora.Core.Entities;
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

    public async Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        // Log to structured logging
        _logger.LogInformation(
            "Audit: {Action} {EntityName}/{EntityId} by {ChangedBy} (Tenant: {TenantId})",
            entry.Action,
            entry.EntityName,
            entry.EntityId,
            entry.ChangedBy,
            entry.TenantId);

        // Implement persistent audit log storage
        if (_context != null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    TenantId = entry.TenantId,
                    EntityName = entry.EntityName,
                    EntityId = entry.EntityId,
                    Action = (Execora.Core.Enums.AuditAction)entry.Action,
                    ActionType = entry.ActionType,
                    OldValues = entry.OldValues,
                    NewValues = entry.NewValues,
                    ChangedBy = entry.ChangedBy,
                    ChangedByType = "API", // Default to API for now, can be enhanced later
                    IpAddress = entry.IpAddress,
                    UserAgent = entry.UserAgent,
                    ProjectId = entry.ProjectId,
                    ChangedAt = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogDebug("Audit log persisted to database for {Action} {EntityName}/{EntityId}", entry.Action, entry.EntityName, entry.EntityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist audit log to database. Falling back to structured logging.");
                // Continue with structured logging as fallback
            }
        }
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
            Action = Execora.Core.Enums.AuditAction.Created,
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
            Action = Execora.Core.Enums.AuditAction.Updated,
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
            Action = Execora.Core.Enums.AuditAction.Deleted,
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
            Action = Execora.Core.Enums.AuditAction.StateChanged,
            ActionType = $"{oldState} -> {newState}",
            ChangedBy = changedBy,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ProjectId = projectId
        }, cancellationToken);
    }

    public Task LogSecurityEventAsync(
        Execora.Core.Enums.AuditAction action,
        string entityName,
        string? entityId,
        string description,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Security Event: {Action} {EntityName}/{EntityId} - {Description}",
            action,
            entityName,
            entityId ?? "null",
            description);

        return Task.CompletedTask;
    }

    public Task LogErrorAsync(
        string errorMessage,
        string? stackTrace = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogError(
            "Error: {ErrorMessage}\nStackTrace: {StackTrace}\nIP: {IpAddress}\nUserAgent: {UserAgent}",
            errorMessage,
            stackTrace ?? "null",
            ipAddress ?? "null",
            userAgent ?? "null");

        return Task.CompletedTask;
    }
}
