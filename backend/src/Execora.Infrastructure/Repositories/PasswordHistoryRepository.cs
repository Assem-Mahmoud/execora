using Execora.Core.Entities;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Execora.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing password history
/// </summary>
public class PasswordHistoryRepository : IPasswordHistoryRepository
{
    private readonly ExecoraDbContext _context;

    public PasswordHistoryRepository(ExecoraDbContext context)
    {
        _context = context;
    }

    public async Task AddPasswordAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default)
    {
        var historyEntry = new PasswordHistory
        {
            UserId = userId,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.PasswordHistory.Add(historyEntry);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<string>> GetRecentPasswordHashesAsync(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordHistory
            .Where(ph => ph.UserId == userId)
            .OrderByDescending(ph => ph.CreatedAt)
            .Take(count)
            .Select(ph => ph.PasswordHash)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPasswordInHistoryAsync(Guid userId, string passwordHash, int historyCount, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordHistory
            .Where(ph => ph.UserId == userId)
            .OrderByDescending(ph => ph.CreatedAt)
            .Take(historyCount)
            .AnyAsync(ph => ph.PasswordHash == passwordHash, cancellationToken);
    }

    public async Task CleanupOldHistoryAsync(Guid userId, int keepCount, CancellationToken cancellationToken = default)
    {
        var oldEntries = await _context.PasswordHistory
            .Where(ph => ph.UserId == userId)
            .OrderByDescending(ph => ph.CreatedAt)
            .Skip(keepCount)
            .ToListAsync(cancellationToken);

        if (oldEntries.Any())
        {
            _context.PasswordHistory.RemoveRange(oldEntries);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
