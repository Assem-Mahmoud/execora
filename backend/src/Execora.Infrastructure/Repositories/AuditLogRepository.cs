using Execora.Core.Entities;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Execora.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing audit logs
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly ExecoraDbContext _context;

    public AuditLogRepository(ExecoraDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);
        return auditLog;
    }

    public async Task<List<AuditLog>> GetByTenantIdAsync(Guid tenantId, int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(al => al.TenantId == tenantId)
            .OrderByDescending(al => al.ChangedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(al => al.EntityName == entityName && al.EntityId == entityId)
            .OrderByDescending(al => al.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetByUserAsync(Guid userId, int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(al => al.ChangedBy == userId)
            .OrderByDescending(al => al.ChangedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
