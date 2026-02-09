using Execora.Application.DTOs;
using Execora.Core.Entities;

namespace Execora.Application.Services;

/// <summary>
/// Service for handling email verification flows
/// </summary>
public interface IEmailVerificationService
{
    /// <summary>
    /// Generates a new verification token for a user
    /// </summary>
    /// <param name="email">The email address to generate token for</param>
    /// <returns>The generated verification token</returns>
    Task<EmailVerificationToken> GenerateVerificationTokenAsync(string email);

    /// <summary>
    /// Verifies an email using the provided token
    /// </summary>
    /// <param name="token">The verification token</param>
    /// <returns>Verification result</returns>
    Task<VerifyEmailResponse> VerifyEmailAsync(string token);

    /// <summary>
    /// Resends a verification email to a user
    /// </summary>
    /// <param name="email">The email address to send verification to</param>
    /// <returns>Resend result</returns>
    Task<ResendVerificationResponse> ResendVerificationEmailAsync(string email);

    /// <summary>
    /// Gets the verification status for a user
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <returns>Verification status</returns>
    Task<VerificationStatusResponse> GetVerificationStatusAsync(string email);

    /// <summary>
    /// Clean up expired verification tokens
    /// </summary>
    /// <returns>Number of expired tokens cleaned up</returns>
    Task<int> CleanupExpiredTokensAsync();
}

/// <summary>
/// Response model for resending verification email
/// </summary>
public class ResendVerificationResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the resend was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an error message if resend failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the new verification token (if successful)
    /// </summary>
    public string? NewToken { get; set; }

    /// <summary>
    /// Gets or sets a success message if resend succeeded
    /// </summary>
    public string? SuccessMessage { get; set; }
}

/// <summary>
/// Response model for verification status
/// </summary>
public class VerificationStatusResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the email is verified
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Gets or sets the verification status message
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether there's a pending verification
    /// </summary>
    public bool HasPendingVerification { get; set; }

    /// <summary>
    /// Gets or sets the expiration time of the current verification token (if any)
    /// </summary>
    public DateTime? TokenExpiresAt { get; set; }
}