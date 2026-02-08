namespace Execora.Application.DTOs;

/// <summary>
/// Request DTO for user registration with tenant creation
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// User's password
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// User's first name
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// User's last name
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Organization name for the new tenant
    /// </summary>
    public required string OrganizationName { get; init; }
}