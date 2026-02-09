namespace Execora.Core.Entities;

/// <summary>
/// Represents a user in the system. Users can belong to multiple tenants.
/// </summary>
public class User : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// User email address (unique across system)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Email verification status
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Mobile number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Phone verification status
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Account active flag
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last successful login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Last password change timestamp
    /// </summary>
    public DateTime? PasswordChangedAt { get; set; }

    // Navigation properties
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
}
