namespace Execora.Application.DTOs;

/// <summary>
/// Request model for email verification
/// </summary>
public class VerifyEmailRequest
{
    /// <summary>
    /// Gets or sets the verification token from the email link
    /// </summary>
    public required string Token { get; set; }
}