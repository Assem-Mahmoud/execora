using Execora.Core.Entities;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Execora.Infrastructure.Repositories;

/// <summary>
/// Repository for Tenant entity operations
/// </summary>
public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ExecoraDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
    }

    public async Task<Tenant?> GetWithUsersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.TenantUsers)
            .ThenInclude(tu => tu.User)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(t => t.Slug == slug, cancellationToken);
    }

    public async Task<bool> HasReachedProjectLimitAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant == null || !tenant.MaxProjects.HasValue)
        {
            return false;
        }

        // TODO: Implement project count check when Project entity is created
        // For now, return false as projects don't exist yet
        return false;
    }

    public async Task<bool> HasReachedUserLimitAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _dbSet
            .Include(t => t.TenantUsers)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant == null || !tenant.MaxUsers.HasValue)
        {
            return false;
        }

        var activeUserCount = tenant.TenantUsers.Count(tu => tu.IsActive);
        return activeUserCount >= tenant.MaxUsers.Value;
    }
}
