using System.Security.Cryptography;
using System.Text;
using Execora.Core.Entities;
using Execora.Core.Interfaces;

namespace Execora.Auth.Services;

/// <summary>
/// Service for managing refresh token rotation with database persistence
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token for a user
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(Guid userId, bool rememberMe = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token and returns the associated token entity if valid
    /// </summary>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Cleans up expired tokens
    /// </summary>
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Database-backed refresh token service implementation
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _tokenRepository;
    private readonly int _standardExpirationDays = 7;
    private readonly int _rememberMeExpirationDays = 30;

    public RefreshTokenService(IRefreshTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, bool rememberMe = false, CancellationToken cancellationToken = default)
    {
        var token = GenerateSecureToken();
        var tokenHash = HashToken(token);
        var expiresAt = DateTime.UtcNow.AddDays(rememberMe ? _rememberMeExpirationDays : _standardExpirationDays);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            DeviceIdentifier = null, // Could be extracted from request
            IsRevoked = false,
            RememberMe = rememberMe
        };

        await _tokenRepository.CreateAsync(refreshToken, cancellationToken);
        return token;
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var tokenHash = HashToken(token);
        var storedToken = await _tokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        return storedToken;
    }

    public async Task<string?> RotateRefreshTokenAsync(string oldToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await ValidateRefreshTokenAsync(oldToken, cancellationToken);

        if (storedToken == null)
        {
            return null;
        }

        // Revoke old token
        await _tokenRepository.RevokeAsync(storedToken, cancellationToken);

        // Generate new token
        return await GenerateRefreshTokenAsync(storedToken.UserId, storedToken.RememberMe, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var storedToken = await ValidateRefreshTokenAsync(token, cancellationToken);

        if (storedToken != null)
        {
            await _tokenRepository.RevokeAsync(storedToken, cancellationToken);
        }
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _tokenRepository.RevokeAllForUserAsync(userId, cancellationToken);
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        await _tokenRepository.DeleteExpiredAsync(cancellationToken);
    }

    /// <summary>
    /// Generates a cryptographically secure random token
    /// </summary>
    private static string GenerateSecureToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Hashes a token using SHA-256
    /// </summary>
    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
