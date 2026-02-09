namespace Execora.Core.Entities;

/// <summary>
/// Represents an email verification token for user account activation
/// </summary>
public class EmailVerificationToken : BaseEntity
{
    /// <summary>
    /// Gets or sets the email address this token is associated with
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the verification token (hashed for security)
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for this verification token
    /// </summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this token has been used
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with this token (nullable for invitation flows)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID associated with this token (nullable for invitation flows)
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the date when the token was used
    /// </summary>
    public DateTime? UsedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}