namespace Execora.Infrastructure.Services.Email;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email verification email
    /// </summary>
    Task SendEmailVerificationAsync(string email, string verificationToken, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset email
    /// </summary>
    Task SendPasswordResetAsync(string email, string resetToken, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an invitation email
    /// </summary>
    Task SendInvitationAsync(string email, string invitationToken, string tenantName, string inviterName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a welcome email after registration
    /// </summary>
    Task SendWelcomeEmailAsync(string email, string userName, string tenantName, CancellationToken cancellationToken = default);
}
