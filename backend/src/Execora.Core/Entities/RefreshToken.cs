namespace Execora.Core.Entities;

/// <summary>
/// Represents a refresh token used for maintaining user sessions.
/// Tokens are stored hashed using SHA-256.
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// User who owns this token
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// SHA-256 hash of the token
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration datetime
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Optional device/browser fingerprint
    /// </summary>
    public string? DeviceIdentifier { get; set; }

    /// <summary>
    /// Revocation status
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// When token was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Extended lifetime flag (30 days vs 7 days)
    /// </summary>
    public bool RememberMe { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
