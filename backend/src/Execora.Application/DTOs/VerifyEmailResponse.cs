namespace Execora.Application.DTOs;

/// <summary>
/// Response model for email verification
/// </summary>
public class VerifyEmailResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the email was successfully verified
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the email is now verified
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Gets or sets an error message if verification failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets a success message if verification succeeded
    /// </summary>
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Gets or sets the email address that was verified
    /// </summary>
    public string? Email { get; set; }
}