using Execora.Application.DTOs;
using Execora.Core.Entities;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Services;
using Execora.Infrastructure.Services.Email;
using Execora.Auth.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Execora.Core.Enums;

namespace Execora.Application.Services;

public class PasswordResetService : IPasswordResetService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly IAuditLogService _auditLogService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordHistoryRepository _passwordHistoryRepository;

    public PasswordResetService(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IEmailService emailService,
        ITokenService tokenService,
        IAuditLogService auditLogService,
        IPasswordHasher passwordHasher,
        IPasswordHistoryRepository passwordHistoryRepository)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _emailService = emailService;
        _tokenService = tokenService;
        _auditLogService = auditLogService;
        _passwordHasher = passwordHasher;
        _passwordHistoryRepository = passwordHistoryRepository;
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        // Check if user exists with this email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !user.EmailVerified || !user.IsActive)
        {
            // For security, don't reveal if the email exists
            // Log the attempt but don't send email
            await _auditLogService.LogSecurityEventAsync(
                AuditAction.PasswordResetRequested,
                "User",
                null,
                $"Password reset requested for email: {request.Email}",
                null,
                null);
            return;
        }

        // Get user's tenant for email template
        // For password reset, we'll use the user's first active tenant
        var tenantUser = await _userRepository.GetWithTenantsAsync(user.Id);
        if (tenantUser?.TenantUsers == null || !tenantUser.TenantUsers.Any())
        {
            throw new InvalidOperationException("User is not associated with any tenant");
        }

        var tenant = tenantUser.TenantUsers
            .Where(tu => tu.IsActive)
            .Select(tu => tu.Tenant)
            .FirstOrDefault();

        if (tenant == null)
        {
            throw new InvalidOperationException("User does not have an active tenant membership");
        }

        // Generate secure reset token
        var resetToken = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.AddHours(1); // 1 hour expiration

        var passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = resetToken,
            ExpiresAt = expiresAt,
            IpAddress = "", // IP and UserAgent will be set by controller before creating
            UserAgent = ""
        };

        await _passwordResetTokenRepository.CreateAsync(passwordResetToken);

        // Send reset email
        await _emailService.SendPasswordResetAsync(
            user.Email,
            passwordResetToken.Token,
            $"{user.FirstName} {user.LastName}");

        // Log the reset request
        await _auditLogService.LogSecurityEventAsync(
            AuditAction.PasswordResetRequested,
            "User",
            user.Id.ToString(),
            $"Password reset requested for user: {user.Email}",
            null,
            null);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        // Find valid reset token
        var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(request.Token);

        if (resetToken == null)
        {
            throw new InvalidOperationException("Invalid or expired reset token");
        }

        // Get user associated with token
        var user = await _userRepository.GetByIdAsync(resetToken.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Validate new password requirements
        ValidatePassword(request.NewPassword);

        // Hash new password
        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        // Check if password is in history (last 5 passwords)
        var passwordHistory = await GetPasswordHistoryAsync(user.Id, 5);
        if (_passwordHasher.IsPasswordInHistory(newPasswordHash, passwordHistory))
        {
            throw new InvalidOperationException("Cannot reuse a previous password");
        }

        // Update user password
        await _userRepository.UpdatePasswordAsync(user.Id, newPasswordHash);

        // Add to password history
        var historyHash = _passwordHasher.AddToPasswordHistory(newPasswordHash);
        await _passwordHistoryRepository.AddPasswordAsync(user.Id, historyHash);

        // Mark token as used
        await _passwordResetTokenRepository.MarkAsUsedAsync(resetToken.Id);

        // Invalidate all existing refresh tokens for this user
        await _tokenService.InvalidateUserRefreshTokensAsync(user.Id);

        // Log the password reset
        await _auditLogService.LogSecurityEventAsync(
            AuditAction.PasswordReset,
            "User",
            user.Id.ToString(),
            $"Password reset completed for user: {user.Email}",
            null,
            null);

        // Log successful password reset
        await _auditLogService.LogSecurityEventAsync(
            AuditAction.SecurityEvent,
            "Security",
            null,
            $"Password reset successful for user: {user.Email}",
            null,
            null);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Verify current password
        var isCurrentPasswordValid = _passwordHasher.VerifyPassword(
            request.CurrentPassword,
            user.PasswordHash);

        if (!isCurrentPasswordValid)
        {
            // Log failed password change attempt
            await _auditLogService.LogSecurityEventAsync(
                AuditAction.PasswordChangeFailed,
                "User",
                user.Id.ToString(),
                $"Failed password change attempt for user: {user.Email}",
                null,
                null);

            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        // Validate new password requirements
        ValidatePassword(request.NewPassword);

        // Hash new password
        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        // Check if password is in history (last 5 passwords)
        var passwordHistory = await GetPasswordHistoryAsync(user.Id, 5);
        if (_passwordHasher.IsPasswordInHistory(newPasswordHash, passwordHistory))
        {
            throw new InvalidOperationException("Cannot reuse a recent password");
        }

        // Update user password
        await _userRepository.UpdatePasswordAsync(user.Id, newPasswordHash);

        // Add to password history
        var historyHash = _passwordHasher.AddToPasswordHistory(newPasswordHash);
        await _passwordHistoryRepository.AddPasswordAsync(user.Id, historyHash);

        // Invalidate all existing refresh tokens for this user
        await _tokenService.InvalidateUserRefreshTokensAsync(user.Id);

        // Log the password change
        await _auditLogService.LogSecurityEventAsync(
            AuditAction.PasswordChanged,
            "User",
            user.Id.ToString(),
            "Password changed successfully",
            null,
            null);

        // Log successful password change
        await _auditLogService.LogSecurityEventAsync(
            AuditAction.SecurityEvent,
            "Security",
            null,
            $"Password change successful for user: {user.Email}",
            null,
            null);
    }

    private string GenerateSecureToken()
    {
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        randomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty");
        }

        if (password.Length < 12)
        {
            throw new ArgumentException("Password must be at least 12 characters long");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter");
        }

        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least one digit");
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException("Password must contain at least one special character");
        }
    }

    private async Task<List<string>> GetPasswordHistoryAsync(Guid userId, int count)
    {
        return await _passwordHistoryRepository.GetRecentPasswordHashesAsync(userId, count);
    }
}