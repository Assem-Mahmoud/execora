using Execora.Core.Entities;
using Execora.Core.Enums;

namespace Execora.Auth.Services;

/// <summary>
/// Token service interface for JWT and refresh token operations
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the user
    /// </summary>
    string GenerateAccessToken(User user, Tenant tenant, TenantRole role);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a refresh token
    /// </summary>
    bool ValidateRefreshToken(string token);

    /// <summary>
    /// Gets the expiration date for access tokens
    /// </summary>
    DateTime GetAccessTokenExpiration();

    /// <summary>
    /// Gets the expiration date for refresh tokens
    /// </summary>
    DateTime GetRefreshTokenExpiration();
}
