using Execora.Core.Entities;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Execora.Infrastructure.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly ExecoraDbContext _context;

    public PasswordResetTokenRepository(ExecoraDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        return await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task CreateAsync(PasswordResetToken token)
    {
        await _context.PasswordResetTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAsUsedAsync(Guid tokenId)
    {
        var token = await _context.PasswordResetTokens.FindAsync(tokenId);
        if (token != null)
        {
            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteExpiredTokensAsync()
    {
        var expiredTokens = _context.PasswordResetTokens
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ToList();

        _context.PasswordResetTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var tokens = _context.PasswordResetTokens
            .Where(t => t.UserId == userId)
            .ToList();

        _context.PasswordResetTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }
}