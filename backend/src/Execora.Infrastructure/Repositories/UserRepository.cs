using Execora.Core.Entities;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Execora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Repository for User entity operations
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ExecoraDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetWithTenantsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.TenantUsers)
            .ThenInclude(tu => tu.Tenant)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetWithTenantAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.TenantUsers.Where(tu => tu.TenantId == tenantId))
            .ThenInclude(tu => tu.Tenant)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddTenantUserAsync(TenantUser tenantUser, CancellationToken cancellationToken = default)
    {
        await _context.TenantUsers.AddAsync(tenantUser, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePasswordAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
        }

        var user = await _dbSet.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        user.PasswordHash = passwordHash;
        user.PasswordChangedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
