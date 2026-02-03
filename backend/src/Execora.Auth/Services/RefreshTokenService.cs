using Execora.Auth.Services;

namespace Execora.Auth.Services;

/// <summary>
/// Service for managing refresh token rotation
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token for a user
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token and returns the associated user if valid
    /// </summary>
    Task<(bool IsValid, Guid? UserId, Guid? TenantId)> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotates a refresh token (invalidates old, issues new)
    /// </summary>
    Task<string?> RotateRefreshTokenAsync(string oldToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// In-memory refresh token service for Phase 2
/// Note: In production, this should be replaced with a database-backed implementation
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly Dictionary<string, (Guid UserId, Guid TenantId, DateTime ExpiresAt)> _refreshTokens = new();
    private readonly ITokenService _tokenService;

    public RefreshTokenService(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var token = _tokenService.GenerateRefreshToken();
        var expiresAt = _tokenService.GetRefreshTokenExpiration();

        _refreshTokens[token] = (userId, tenantId, expiresAt);

        return await Task.FromResult(token);
    }

    public async Task<(bool IsValid, Guid? UserId, Guid? TenantId)> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        if (!_refreshTokens.TryGetValue(token, out var tokenData))
        {
            return (false, null, null);
        }

        if (tokenData.ExpiresAt < DateTime.UtcNow)
        {
            _refreshTokens.Remove(token);
            return (false, null, null);
        }

        return (true, tokenData.UserId, tokenData.TenantId);
    }

    public async Task<string?> RotateRefreshTokenAsync(string oldToken, CancellationToken cancellationToken = default)
    {
        var (isValid, userId, tenantId) = await ValidateRefreshTokenAsync(oldToken, cancellationToken);

        if (!isValid || userId == null || tenantId == null)
        {
            return null;
        }

        // Revoke old token
        await RevokeRefreshTokenAsync(oldToken, cancellationToken);

        // Generate new token
        return await GenerateRefreshTokenAsync(userId.Value, tenantId.Value, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        _refreshTokens.Remove(token);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var tokensToRemove = _refreshTokens
            .Where(kvp => kvp.Value.UserId == userId)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var token in tokensToRemove)
        {
            _refreshTokens.Remove(token);
        }
    }
}
