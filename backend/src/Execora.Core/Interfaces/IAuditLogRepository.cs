using Execora.Core.Entities;

namespace Execora.Core.Interfaces;

/// <summary>
/// Repository for managing audit logs
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Creates a new audit log entry
    /// </summary>
    Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs for a tenant
    /// </summary>
    Task<List<AuditLog>> GetByTenantIdAsync(Guid tenantId, int skip = 0, int take = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs for an entity
    /// </summary>
    Task<List<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs by user
    /// </summary>
    Task<List<AuditLog>> GetByUserAsync(Guid userId, int skip = 0, int take = 100, CancellationToken cancellationToken = default);
}
