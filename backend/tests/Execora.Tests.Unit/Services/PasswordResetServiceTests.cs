using Execora.Application.DTOs;
using Execora.Application.Services;
using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Repositories;
using Moq;
using Execora.Infrastructure.Services.Email;
using Execora.Auth.Services;

namespace Execora.Tests.Unit.Services;

public class PasswordResetServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<ITenantRepository> _mockTenantRepository = null!;
    private Mock<IPasswordResetTokenRepository> _mockPasswordResetTokenRepository = null!;
    private Mock<IEmailService> _mockEmailService = null!;
    private Mock<ITokenService> _mockTokenService = null!;
    private Mock<IAuditLogService> _mockAuditLogService = null!;
    private PasswordResetService _passwordResetService = null!;
    private Mock<IPasswordHasher> _mockPasswordHasher = null!;

    public PasswordResetServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTenantRepository = new Mock<ITenantRepository>();
        _mockPasswordResetTokenRepository = new Mock<IPasswordResetTokenRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockAuditLogService = new Mock<IAuditLogService>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();

        _passwordResetService = new PasswordResetService(
            _mockUserRepository.Object,
            _mockTenantRepository.Object,
            _mockPasswordResetTokenRepository.Object,
            _mockEmailService.Object,
            _mockTokenService.Object,
            _mockAuditLogService.Object,
            _mockPasswordHasher.Object
        );
    }

    [Fact]
    public async Task ForgotPassword_WithValidEmail_ShouldGenerateResetToken()
    {
        // Arrange
        var request = new ForgotPasswordRequest { Email = "test@example.com" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsEmailVerified = true,
            IsActive = true,
            TenantId = Guid.NewGuid()
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(user);
        _mockTenantRepository.Setup(x => x.GetByIdAsync(user.TenantId))
            .ReturnsAsync(new Tenant { Name = "Test Tenant" });

        // Act
        await _passwordResetService.ForgotPasswordAsync(request);

        // Assert
        _mockPasswordResetTokenRepository.Verify(x => x.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Once);
        _mockEmailService.Verify(x => x.SendPasswordResetEmailAsync(
            It.Is<PasswordResetToken>(t => t.UserId == user.Id)), Times.Once);
    }

    [Fact]
    public async Task ForgotPassword_WithNonExistentEmail_ShouldNotFail()
    {
        // Arrange
        var request = new ForgotPasswordRequest { Email = "nonexistent@example.com" };

        _mockUserRepository.Setup(x => x.GetByEmailAsync("nonexistent@example.com"))
            .ReturnsAsync((User?)null);

        // Act
        await _passwordResetService.ForgotPasswordAsync(request);

        // Assert - Should not throw exception
        _mockPasswordResetTokenRepository.Verify(x => x.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Never);
        _mockEmailService.Verify(x => x.SendPasswordResetEmailAsync(It.IsAny<PasswordResetToken>()), Times.Never);
    }

    [Fact]
    public async Task ResetPassword_WithValidToken_ShouldUpdatePassword()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Token = "valid-token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = "valid-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false
        };

        var user = new User
        {
            Id = token.UserId,
            Email = "test@example.com",
            PasswordHash = "oldhash"
        };

        _mockPasswordResetTokenRepository.Setup(x => x.GetByTokenAsync("valid-token"))
            .ReturnsAsync(token);
        _mockUserRepository.Setup(x => x.GetByIdAsync(token.UserId))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.HashPassword("NewPassword123!"))
            .Returns("newhash");

        // Act
        await _passwordResetService.ResetPasswordAsync(request);

        // Assert
        _mockUserRepository.Verify(x => x.UpdatePasswordAsync(token.UserId, "newhash"), Times.Once);
        _mockPasswordResetTokenRepository.Verify(x => x.MarkAsUsedAsync(token.Id), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Token = "invalid-token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        _mockPasswordResetTokenRepository.Setup(x => x.GetByTokenAsync("invalid-token"))
            .ReturnsAsync((PasswordResetToken?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _passwordResetService.ResetPasswordAsync(request));
    }

    [Fact]
    public async Task ResetPassword_WithExpiredToken_ShouldThrowException()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Token = "expired-token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = "expired-token",
            ExpiresAt = DateTime.UtcNow.AddHours(-1),
            IsUsed = false
        };

        _mockPasswordResetTokenRepository.Setup(x => x.GetByTokenAsync("expired-token"))
            .ReturnsAsync(token);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _passwordResetService.ResetPasswordAsync(request));
    }

    [Fact]
    public async Task ResetPassword_WithUsedToken_ShouldThrowException()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Token = "used-token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = "used-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = true
        };

        _mockPasswordResetTokenRepository.Setup(x => x.GetByTokenAsync("used-token"))
            .ReturnsAsync(token);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _passwordResetService.ResetPasswordAsync(request));
    }

    [Fact]
    public async Task ChangePassword_WithValidCurrentPassword_ShouldUpdatePassword()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "CurrentPassword123!",
            NewPassword = "NewPassword456!",
            ConfirmPassword = "NewPassword456!"
        };

        var user = new User
        {
            Id = userId,
            PasswordHash = "currenthash"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.VerifyPassword("CurrentPassword123!", "currenthash"))
            .Returns(true);
        _mockPasswordHasher.Setup(x => x.HashPassword("NewPassword456!"))
            .Returns("newhash");
        _mockPasswordHasher.Setup(x => x.IsPasswordInHistory(userId, "NewPassword456!"))
            .Returns(false);

        // Act
        await _passwordResetService.ChangePasswordAsync(userId, request);

        // Assert
        _mockUserRepository.Verify(x => x.UpdatePasswordAsync(userId, "newhash"), Times.Once);
        _mockPasswordHasher.Verify(x => x.AddToPasswordHistory(userId, "newhash"), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidCurrentPassword_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword!",
            NewPassword = "NewPassword456!",
            ConfirmPassword = "NewPassword456!"
        };

        var user = new User
        {
            Id = userId,
            PasswordHash = "currenthash"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.VerifyPassword("WrongPassword!", "currenthash"))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _passwordResetService.ChangePasswordAsync(userId, request));
    }

    [Fact]
    public async Task ChangePassword_WithReusedPassword_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "CurrentPassword123!",
            NewPassword = "CurrentPassword123!", // Same as current
            ConfirmPassword = "CurrentPassword123!"
        };

        var user = new User
        {
            Id = userId,
            PasswordHash = "currenthash"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.VerifyPassword("CurrentPassword123!", "currenthash"))
            .Returns(true);
        _mockPasswordHasher.Setup(x => x.IsPasswordInHistory(userId, "CurrentPassword123!"))
            .Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _passwordResetService.ChangePasswordAsync(userId, request));
    }
}