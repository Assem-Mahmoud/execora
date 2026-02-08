using Execora.Core.Entities;

namespace Execora.Core.Interfaces;

/// <summary>
/// Repository for User entity operations
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets user by email
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user with tenant memberships
    /// </summary>
    Task<User?> GetWithTenantsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user with a specific tenant membership
    /// </summary>
    Task<User?> GetWithTenantAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if email is already in use
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a tenant-user relationship
    /// </summary>
    Task AddTenantUserAsync(TenantUser tenantUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user entity
    /// </summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
