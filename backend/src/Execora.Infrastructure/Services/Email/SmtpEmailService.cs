using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Execora.Infrastructure.Services.Email;

/// <summary>
/// SMTP-based email service implementation
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration configuration;
    private readonly ILogger<SmtpEmailService> logger;
    private readonly string fromEmail;
    private readonly string fromName;
    private readonly string smtpServer;
    private readonly int smtpPort;
    private readonly string smtpUsername;
    private readonly string smtpPassword;
    private readonly bool enableSsl;
    private readonly bool isEnabled;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        this.configuration = configuration;
        this.logger = logger;

        var emailSection = configuration.GetSection("Email");
        isEnabled = bool.TryParse(emailSection["Enabled"], out var enabled) && enabled;
        fromEmail = emailSection["FromEmail"] ?? "noreply@execora.com";
        fromName = emailSection["FromName"] ?? "EXECORA";
        smtpServer = emailSection["SmtpServer"] ?? string.Empty;
        smtpPort = int.TryParse(emailSection["SmtpPort"], out var port) ? port : 587;
        smtpUsername = emailSection["SmtpUsername"] ?? string.Empty;
        smtpPassword = emailSection["SmtpPassword"] ?? string.Empty;
        enableSsl = bool.TryParse(emailSection["EnableSsl"], out var ssl) && ssl;
    }

    public async Task SendEmailVerificationAsync(string email, string verificationToken, string userName, CancellationToken cancellationToken = default)
    {
        var subject = "Verify Your EXECORA Email Address";
        var verificationUrl = $"{GetBaseUrl()}/auth/verify-email?token={verificationToken}";
        var body = $@"
<html>
<body>
    <h2>Welcome to EXECORA, {userName}!</h2>
    <p>Thank you for registering. Please verify your email address by clicking the link below:</p>
    <p><a href='{verificationUrl}'>Verify Email Address</a></p>
    <p>Or copy and paste this link into your browser:</p>
    <p>{verificationUrl}</p>
    <p>This link will expire in 24 hours.</p>
    <hr>
    <p><small>If you didn't create an account with EXECORA, please ignore this email.</small></p>
</body>
</html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetAsync(string email, string resetToken, string userName, CancellationToken cancellationToken = default)
    {
        var subject = "Reset Your EXECORA Password";
        var resetUrl = $"{GetBaseUrl()}/auth/reset-password?token={resetToken}";
        var body = $@"
<html>
<body>
    <h2>Password Reset Request</h2>
    <p>Hi {userName},</p>
    <p>We received a request to reset your password. Click the link below to create a new password:</p>
    <p><a href='{resetUrl}'>Reset Password</a></p>
    <p>Or copy and paste this link into your browser:</p>
    <p>{resetUrl}</p>
    <p>This link will expire in 1 hour.</p>
    <hr>
    <p><small>If you didn't request a password reset, please ignore this email.</small></p>
</body>
</html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendInvitationAsync(string email, string invitationToken, string tenantName, string inviterName, CancellationToken cancellationToken = default)
    {
        var subject = $"You're invited to join {tenantName} on EXECORA";
        var invitationUrl = $"{GetBaseUrl()}/auth/accept-invitation?token={invitationToken}";
        var body = $@"
<html>
<body>
    <h2>Invitation to Join {tenantName}</h2>
    <p>{inviterName} has invited you to join their organization on EXECORA.</p>
    <p>Click the link below to accept the invitation:</p>
    <p><a href='{invitationUrl}'>Accept Invitation</a></p>
    <p>Or copy and paste this link into your browser:</p>
    <p>{invitationUrl}</p>
    <p>This invitation will expire in 7 days.</p>
    <hr>
    <p><small>If you don't want to join this organization, please ignore this email.</small></p>
</body>
</html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string email, string userName, string tenantName, CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to EXECORA!";
        var loginUrl = $"{GetBaseUrl()}/auth/login";
        var body = $@"
<html>
<body>
    <h2>Welcome to EXECORA, {userName}!</h2>
    <p>Your account for <strong>{tenantName}</strong> has been successfully created.</p>
    <p>You can now log in and start using EXECORA:</p>
    <p><a href='{loginUrl}'>Go to Login</a></p>
    <p>We're excited to have you on board!</p>
    <hr>
    <p><small>If you have any questions, don't hesitate to contact our support team.</small></p>
</body>
</html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        if (!isEnabled || string.IsNullOrWhiteSpace(smtpServer))
        {
            logger.LogInformation("Email sending is disabled. Would have sent email to {Email}", toEmail);
            logger.LogDebug("Email Subject: {Subject}, Body: {Body}", subject, body);
            return;
        }

        try
        {
            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage, cancellationToken);
            logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }

    private string GetBaseUrl()
    {
        return configuration["Application:BaseUrl"] ?? "https://localhost:4200";
    }
}
