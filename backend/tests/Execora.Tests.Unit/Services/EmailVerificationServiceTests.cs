using Execora.Application.Services;
using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Services.Email;

namespace Execora.Tests.Unit.Services;

public class EmailVerificationServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IEmailVerificationTokenRepository> _tokenRepositoryMock = null!;
    private Mock<IEmailService> _emailServiceMock = null!;
    private Mock<ILogger<EmailVerificationService>> _loggerMock = null!;
    private EmailVerificationService _service = null!;

    public EmailVerificationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenRepositoryMock = new Mock<IEmailVerificationTokenRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<EmailVerificationService>>();
        _service = new EmailVerificationService(
            _userRepositoryMock.Object,
            _tokenRepositoryMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GenerateVerificationTokenAsync_ShouldGenerateUniqueToken_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailVerified = false,
            EmailVerificationToken = null,
            EmailVerificationTokenExpiresAt = null
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == "test@example.com" ? user : null);

        // Act
        var token = await _service.GenerateVerificationTokenAsync("test@example.com");

        // Assert
        Assert.NotNull(token);
        // EmailVerificationToken doesn't have an Id property in the current implementation
        Assert.Equal("test@example.com", token.Email);
        Assert.False(token.IsUsed);
        Assert.True(token.ExpiresAt > DateTime.UtcNow);
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<User>(u => u.EmailVerificationToken == token.Token)),
            Times.Once);
    }

    [Fact]
    public async Task GenerateVerificationTokenAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GenerateVerificationTokenAsync("test@example.com"));
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldMarkUserAsVerified_WhenTokenIsValid()
    {
        // Arrange
        var token = new EmailVerificationToken
        {
            Token = "valid-token-123",
            Email = "test@example.com",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailVerified = false,
            EmailVerificationToken = "valid-token-123",
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == "test@example.com" ? user : null);

        _userRepositoryMock.Setup(x => x.GetVerificationTokenAsync("valid-token-123"))
            .ReturnsAsync(token);

        // Act
        var result = await _service.VerifyEmailAsync("valid-token-123");

        // Assert
        Assert.True(result.Success);
        Assert.True(result.EmailVerified);
        Assert.Null(result.ErrorMessage);
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<User>(u => u.EmailVerified && u.EmailVerificationToken == null)),
            Times.Once);
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldReturnError_WhenTokenIsExpired()
    {
        // Arrange
        var token = new EmailVerificationToken
        {
            Token = "expired-token",
            Email = "test@example.com",
            ExpiresAt = DateTime.UtcNow.AddHours(-1),
            IsUsed = false
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailVerified = false,
            EmailVerificationToken = "expired-token",
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(-1)
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == "test@example.com" ? user : null);

        _userRepositoryMock.Setup(x => x.GetVerificationTokenAsync("expired-token"))
            .ReturnsAsync(token);

        // Act
        var result = await _service.VerifyEmailAsync("expired-token");

        // Assert
        Assert.False(result.Success);
        Assert.False(result.EmailVerified);
        Assert.Equal("Verification token has expired", result.ErrorMessage);
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldReturnError_WhenTokenIsAlreadyUsed()
    {
        // Arrange
        var token = new EmailVerificationToken
        {
            Token = "used-token",
            Email = "test@example.com",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailVerified = false,
            EmailVerificationToken = "used-token",
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == "test@example.com" ? user : null);

        _userRepositoryMock.Setup(x => x.GetVerificationTokenAsync("used-token"))
            .ReturnsAsync(token);

        // Act
        var result = await _service.VerifyEmailAsync("used-token");

        // Assert
        Assert.False(result.Success);
        Assert.False(result.EmailVerified);
        Assert.Equal("Verification token has already been used", result.ErrorMessage);
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.VerifyEmailAsync("any-token");

        // Assert
        Assert.False(result.Success);
        Assert.False(result.EmailVerified);
        Assert.Equal("User not found", result.ErrorMessage);
    }

    [Fact]
    public async Task ResendVerificationEmailAsync_ShouldGenerateNewToken_WhenUserExistsAndNotVerified()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailVerified = false,
            EmailVerificationToken = "old-token",
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(-1)
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == "test@example.com" ? user : null);

        // Act
        var result = await _service.ResendVerificationEmailAsync("test@example.com");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.NewToken);
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<User>(u => u.EmailVerificationToken != "old-token")),
            Times.Once);
    }

    [Fact]
    public async Task ResendVerificationEmailAsync_ShouldReturnError_WhenUserAlreadyVerified()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailVerified = true
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == "test@example.com" ? user : null);

        // Act
        var result = await _service.ResendVerificationEmailAsync("test@example.com");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email is already verified", result.ErrorMessage);
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task ResendVerificationEmailAsync_ShReturnError_WhenUserNotFound()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.ResendVerificationEmailAsync("test@example.com");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found", result.ErrorMessage);
    }

    [Fact]
    public async Task GetVerificationStatusAsync_ShouldReturnCorrectStatus_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailVerified = true
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == "test@example.com" ? user : null);

        // Act
        var result = await _service.GetVerificationStatusAsync("test@example.com");

        // Assert
        Assert.True(result.IsVerified);
        Assert.Equal("Email is already verified", result.Status);
    }
}