using Execora.Core.Entities;

namespace Execora.Core.Interfaces;

/// <summary>
/// Repository for Tenant entity operations
/// </summary>
public interface ITenantRepository : IRepository<Tenant>
{
    /// <summary>
    /// Gets tenant by slug
    /// </summary>
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tenant with users
    /// </summary>
    Task<Tenant?> GetWithUsersAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if slug is already in use
    /// </summary>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant has reached its maximum project limit
    /// </summary>
    Task<bool> HasReachedProjectLimitAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant has reached its maximum user limit
    /// </summary>
    Task<bool> HasReachedUserLimitAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
