using Execora.Core.Entities;

namespace Execora.Core.Interfaces;

/// <summary>
/// Repository for managing refresh tokens
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Creates a new refresh token
    /// </summary>
    Task<RefreshToken> CreateAsync(RefreshToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a token by its hash
    /// </summary>
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active tokens for a user
    /// </summary>
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a token
    /// </summary>
    Task<RefreshToken> UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a specific token
    /// </summary>
    Task RevokeAsync(RefreshToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all tokens for a user
    /// </summary>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a token
    /// </summary>
    Task DeleteAsync(RefreshToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired tokens
    /// </summary>
    Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
}
