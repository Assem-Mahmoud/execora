using Execora.Core.Entities;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Execora.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for email verification tokens
/// </summary>
public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly ExecoraDbContext _context;

    public EmailVerificationTokenRepository(ExecoraDbContext context)
    {
        _context = context;
    }

    public async Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
    {
        return await _context.EmailVerificationTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<List<EmailVerificationToken>> GetByEmailAsync(string email)
    {
        return await _context.EmailVerificationTokens
            .Where(t => t.Email == email)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<EmailVerificationToken>> GetByUserIdAsync(Guid userId)
    {
        return await _context.EmailVerificationTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<EmailVerificationToken> UpdateAsync(EmailVerificationToken token)
    {
        _context.Entry(token).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task DeleteAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteExpiredTokensAsync()
    {
        var expiredTokens = await _context.EmailVerificationTokens
            .Where(t => t.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        _context.EmailVerificationTokens.RemoveRange(expiredTokens);
        return await _context.SaveChangesAsync();
    }

    public async Task DeleteByEmailAsync(string email)
    {
        var tokens = await _context.EmailVerificationTokens
            .Where(t => t.Email == email)
            .ToListAsync();

        _context.EmailVerificationTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var tokens = await _context.EmailVerificationTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();

        _context.EmailVerificationTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }
}