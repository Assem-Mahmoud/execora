namespace Execora.Core.Entities;

/// <summary>
/// Tracks password history to prevent reuse of recent passwords
/// </summary>
public class PasswordHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// User who owns this password history entry
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Hashed password (same format as User.PasswordHash)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// When this password was set
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
}
