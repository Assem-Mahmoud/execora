using Execora.Core.Entities;
using Execora.Core.Enums;

namespace Execora.Core.Interfaces;

/// <summary>
/// Repository for managing invitations
/// </summary>
public interface IInvitationRepository
{
    /// <summary>
    /// Creates a new invitation
    /// </summary>
    Task<Invitation> CreateAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an invitation by its ID
    /// </summary>
    Task<Invitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an invitation by its token
    /// </summary>
    Task<Invitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending invitations for a tenant
    /// </summary>
    Task<List<Invitation>> GetPendingByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending invitations for an email address
    /// </summary>
    Task<List<Invitation>> GetPendingByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a pending invitation exists for email and tenant
    /// </summary>
    Task<bool> ExistsPendingAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an invitation
    /// </summary>
    Task<Invitation> UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an invitation
    /// </summary>
    Task DeleteAsync(Invitation invitation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks expired invitations as expired
    /// </summary>
    Task MarkExpiredAsync(CancellationToken cancellationToken = default);
}
