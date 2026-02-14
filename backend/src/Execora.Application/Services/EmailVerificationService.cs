using Execora.Application.DTOs;
using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Services.Email;
using Microsoft.Extensions.Logging;

namespace Execora.Application.Services;

/// <summary>
/// Service for handling email verification flows
/// </summary>
public class EmailVerificationService : IEmailVerificationService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailVerificationService> _logger;
    private readonly TimeSpan _tokenLifetime = TimeSpan.FromHours(24); // 24 hour expiration

    public EmailVerificationService(
        IUserRepository userRepository,
        IEmailVerificationTokenRepository tokenRepository,
        IEmailService emailService,
        ILogger<EmailVerificationService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a new verification token for a user
    /// </summary>
    public async Task<EmailVerificationToken> GenerateVerificationTokenAsync(string email)
    {
        _logger.LogInformation("Generating verification token for email: {Email}", email);

        // Clean up any existing tokens for this email
        await _tokenRepository.DeleteByEmailAsync(email);

        var user = await _userRepository.GetByEmailAsync(email) ??
            throw new KeyNotFoundException($"User with email {email} not found");

        if (user.EmailVerified)
        {
            _logger.LogInformation("Email {Email} is already verified", email);
            throw new InvalidOperationException("Email is already verified");
        }

        // Generate secure token
        var token = Guid.NewGuid().ToString("N");
        var hashedToken = HashToken(token);

        var verificationToken = new EmailVerificationToken
        {
            Email = email,
            Token = hashedToken,
            ExpiresAt = DateTime.UtcNow.Add(_tokenLifetime),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _tokenRepository.CreateAsync(verificationToken);
        _logger.LogInformation("Generated verification token for email: {Email}", email);

        // Return the unhashed token for email sending
        verificationToken.Token = token;
        return verificationToken;
    }

    /// <summary>
    /// Verifies an email using the provided token
    /// </summary>
    public async Task<VerifyEmailResponse> VerifyEmailAsync(string token)
    {
        _logger.LogInformation("Verifying email with token");

        try
        {
            // Hash the token for lookup
            var hashedToken = HashToken(token);

            // Get the verification token
            var verificationToken = await _tokenRepository.GetByTokenAsync(hashedToken);
            if (verificationToken == null)
            {
                _logger.LogWarning("Invalid verification token used");
                return new VerifyEmailResponse
                {
                    Success = false,
                    EmailVerified = false,
                    ErrorMessage = "Invalid verification token"
                };
            }

            // Check if token is expired
            if (verificationToken.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Verification token expired for email: {Email}", verificationToken.Email);
                return new VerifyEmailResponse
                {
                    Success = false,
                    EmailVerified = false,
                    ErrorMessage = "Verification token has expired"
                };
            }

            // Check if token is already used
            if (verificationToken.IsUsed)
            {
                _logger.LogWarning("Verification token already used for email: {Email}", verificationToken.Email);
                return new VerifyEmailResponse
                {
                    Success = false,
                    EmailVerified = false,
                    ErrorMessage = "Verification token has already been used"
                };
            }

            // Get the user
            var user = await _userRepository.GetByEmailAsync(verificationToken.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", verificationToken.Email);
                return new VerifyEmailResponse
                {
                    Success = false,
                    EmailVerified = false,
                    ErrorMessage = "User not found"
                };
            }

            // Mark token as used FIRST to prevent race condition
            // This ensures concurrent requests will fail the IsUsed check above
            verificationToken.IsUsed = true;
            verificationToken.UsedAt = DateTime.UtcNow;
            await _tokenRepository.UpdateAsync(verificationToken);

            // Mark user as verified
            user.EmailVerified = true;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Email verified successfully for: {Email}", verificationToken.Email);

            return new VerifyEmailResponse
            {
                Success = true,
                EmailVerified = true,
                SuccessMessage = "Email verified successfully",
                Email = verificationToken.Email
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification");
            return new VerifyEmailResponse
            {
                Success = false,
                EmailVerified = false,
                ErrorMessage = "An error occurred during verification"
            };
        }
    }

    /// <summary>
    /// Resends a verification email to a user
    /// </summary>
    public async Task<ResendVerificationResponse> ResendVerificationEmailAsync(string email)
    {
        _logger.LogInformation("Resending verification email to: {Email}", email);

        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                return new ResendVerificationResponse
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            if (user.EmailVerified)
            {
                _logger.LogWarning("Email {Email} is already verified", email);
                return new ResendVerificationResponse
                {
                    Success = false,
                    ErrorMessage = "Email is already verified"
                };
            }

            // Generate new token
            var verificationToken = await GenerateVerificationTokenAsync(email);

            // Send email
            await SendVerificationEmailAsync(email, verificationToken.Token);

            _logger.LogInformation("Verification email resent to: {Email}", email);

            return new ResendVerificationResponse
            {
                Success = true,
                SuccessMessage = "Verification email resent successfully",
                NewToken = verificationToken.Token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending verification email to: {Email}", email);
            return new ResendVerificationResponse
            {
                Success = false,
                ErrorMessage = "Failed to resend verification email"
            };
        }
    }

    /// <summary>
    /// Gets the verification status for a user
    /// </summary>
    public async Task<VerificationStatusResponse> GetVerificationStatusAsync(string email)
    {
        _logger.LogDebug("Getting verification status for email: {Email}", email);

        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new VerificationStatusResponse
                {
                    IsVerified = false,
                    Status = "User not found",
                    Email = email
                };
            }

            var response = new VerificationStatusResponse
            {
                IsVerified = user.EmailVerified,
                Email = email
            };

            if (user.EmailVerified)
            {
                response.Status = "Email is already verified";
            }
            else
            {
                response.Status = "Email verification pending";
                response.HasPendingVerification = true;

                // Check if there's an active token
                var tokens = await _tokenRepository.GetByEmailAsync(email);
                var activeToken = tokens.FirstOrDefault(t => !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
                if (activeToken != null)
                {
                    response.TokenExpiresAt = activeToken.ExpiresAt;
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification status for email: {Email}", email);
            return new VerificationStatusResponse
            {
                IsVerified = false,
                Status = "Error checking verification status",
                Email = email
            };
        }
    }

    /// <summary>
    /// Clean up expired verification tokens
    /// </summary>
    public async Task<int> CleanupExpiredTokensAsync()
    {
        _logger.LogInformation("Cleaning up expired verification tokens");
        var count = await _tokenRepository.DeleteExpiredTokensAsync();
        _logger.LogInformation("Cleaned up {Count} expired verification tokens", count);
        return count;
    }

    /// <summary>
    /// Hashes a token for secure storage
    /// </summary>
    private string HashToken(string token)
    {
        // Using SHA-256 for hashing - in production, consider Argon2 or similar
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Sends verification email to the user
    /// </summary>
    private async Task SendVerificationEmailAsync(string email, string token)
    {
        var userName = "User"; // In a real implementation, get user's name
        await _emailService.SendEmailVerificationAsync(email, token, userName);
    }
}