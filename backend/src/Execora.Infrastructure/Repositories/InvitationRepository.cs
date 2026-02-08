using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Execora.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing invitations
/// </summary>
public class InvitationRepository : IInvitationRepository
{
    private readonly ExecoraDbContext _context;

    public InvitationRepository(ExecoraDbContext context)
    {
        _context = context;
    }

    public async Task<Invitation> CreateAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);
        return invitation;
    }

    public async Task<Invitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Invitations
            .Include(i => i.Tenant)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Invitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Invitations
            .Include(i => i.Tenant)
            .FirstOrDefaultAsync(i => i.Token == token, cancellationToken);
    }

    public async Task<List<Invitation>> GetPendingByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Invitations
            .Include(i => i.Tenant)
            .Where(i => i.TenantId == tenantId && i.Status == InvitationStatus.Pending)
            .OrderByDescending(i => i.InvitedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Invitation>> GetPendingByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Invitations
            .Include(i => i.Tenant)
            .Where(i => i.Email == email && i.Status == InvitationStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsPendingAsync(string email, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Invitations
            .AnyAsync(i => i.Email == email &&
                          i.TenantId == tenantId &&
                          i.Status == InvitationStatus.Pending, cancellationToken);
    }

    public async Task<Invitation> UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Update(invitation);
        await _context.SaveChangesAsync(cancellationToken);
        return invitation;
    }

    public async Task DeleteAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Remove(invitation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkExpiredAsync(CancellationToken cancellationToken = default)
    {
        var expiredInvitations = await _context.Invitations
            .Where(i => i.Status == InvitationStatus.Pending && i.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var invitation in expiredInvitations)
        {
            invitation.Status = InvitationStatus.Expired;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
