namespace Execora.Application.DTOs;

/// <summary>
/// Request model for resending verification email
/// </summary>
public class ResendVerificationRequest
{
    /// <summary>
    /// Gets or sets the email address to send verification to
    /// </summary>
    public required string Email { get; set; }
}