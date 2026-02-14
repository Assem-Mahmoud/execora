using Execora.Auth.Services;
using FluentAssertions;
using Xunit;

namespace Execora.Tests.Unit.Services;

public class PasswordHasherTests : IDisposable
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    public void Dispose()
    {
        // Cleanup if needed
    }

    [Fact]
    public void HashPassword_WithValidPassword_ShouldReturnHash()
    {
        // Arrange
        var password = "Password123!";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$2a$"); // BCrypt header
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "Password123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "Password123!";
        var wrongPassword = "WrongPassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "Password123!";
        var emptyPassword = "";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(emptyPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "Password123!";
        var emptyHash = "";

        // Act
        var result = _passwordHasher.VerifyPassword(password, emptyHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithNullPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "Password123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(null!, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithNullHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "Password123!";

        // Act
        var result = _passwordHasher.VerifyPassword(password, null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_WithSamePasswordTwice_ShouldReturnDifferentHashes()
    {
        // Arrange
        var password = "Password123!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBeEquivalentTo(hash2);
    }

    [Fact]
    public void HashPassword_WithEmptyString_ShouldReturnValidHash()
    {
        // Arrange
        var password = "";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2a$");
    }

    [Fact]
    public void HashPassword_WithVeryLongPassword_ShouldReturnValidHash()
    {
        // Arrange
        var longPassword = new string('a', 1000) + "Password123!";

        // Act
        var hash = _passwordHasher.HashPassword(longPassword);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2a$");
    }

    [Fact]
    public void VerifyPassword_WithSpecialCharacters_ShouldWorkCorrectly()
    {
        // Arrange
        var password = "Password@123!#";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithUnicodeCharacters_ShouldWorkCorrectly()
    {
        // Arrange
        var password = "Pässwörd123!你好";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }
}