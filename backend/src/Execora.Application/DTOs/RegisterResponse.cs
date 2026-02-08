namespace Execora.Application.DTOs;

/// <summary>
/// Response DTO for user registration
/// </summary>
public record RegisterResponse
{
    /// <summary>
    /// ID of the newly registered user
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// ID of the newly created tenant
    /// </summary>
    public required Guid TenantId { get; init; }

    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// User's first name
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// User's last name
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Organization name of the tenant
    /// </summary>
    public required string OrganizationName { get; init; }

    /// <summary>
    /// User's role in the tenant
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    /// Email verification status
    /// </summary>
    public bool EmailConfirmed { get; init; }

    /// <summary>
    /// Email verification token (for client verification)
    /// </summary>
    public string? EmailVerificationToken { get; init; }
}