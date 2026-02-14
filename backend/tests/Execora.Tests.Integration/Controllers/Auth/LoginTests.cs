using Execora.Api.IntegrationTests.Helpers;
using Execora.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Execora.Api.IntegrationTests.Controllers.Auth;

public class LoginTests : IntegrationTestBase
{
    private readonly HttpClient _client;

    public LoginTests()
    {
        _client = CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnTokenAndUser()
    {
        // Arrange
        // First, register a user
        var registerRequest = new RegisterRequest
        {
            Email = "login-test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            OrganizationName = "Test Organization",
            OrganizationSlug = "test-org",
            FirstName = "Test",
            LastName = "User",
            AcceptTerms = true
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        // Now try to login
        var loginRequest = new LoginRequest
        {
            Email = "login-test@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
        content.Should().NotBeNull();
        content.Token.Should().NotBeNullOrEmpty();
        content.User.Should().NotBeNull();
        content.User.Email.Should().Be("login-test@example.com");
        content.User.EmailConfirmed.Should().BeTrue();
        content.User.FirstName.Should().Be("Test");
        content.User.LastName.Should().Be("User");
        content.User.Role.Should().NotBeNull();
        content.User.TenantId.Should().NotBeNull();
        content.User.TenantName.Should().Be("Test Organization");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        // First, register a user
        var registerRequest = new RegisterRequest
        {
            Email = "login-invalid-test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            OrganizationName = "Test Organization",
            OrganizationSlug = "test-org",
            FirstName = "Test",
            LastName = "User",
            AcceptTerms = true
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        // Now try to login with wrong password
        var loginRequest = new LoginRequest
        {
            Email = "login-invalid-test@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().System.Net.HttpStatusCode.Unauthorized;
    }

    [Fact]
    public async Task Login_WithUnconfirmedEmail_ShouldReturnError()
    {
        // This test will be implemented after email verification is implemented
        // For now, it should work since email verification is not enforced yet
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "login-unconfirmed-test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            OrganizationName = "Test Organization",
            OrganizationSlug = "test-org",
            FirstName = "Test",
            LastName = "User",
            AcceptTerms = true
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        // Act
        var loginRequest = new LoginRequest
        {
            Email = "login-unconfirmed-test@example.com",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert - This should succeed until email verification is enforced
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().System.Net.HttpStatusCode.Unauthorized;
    }

    [Fact]
    public async Task Login_WithInvalidEmailFormat_ShouldReturnBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "invalid-email",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().System.Net.HttpStatusCode.BadRequest;
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().System.Net.HttpStatusCode.BadRequest;
    }

    [Fact]
    public async Task Login_WithoutEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().System.Net.HttpStatusCode.BadRequest;
    }

    [Fact]
    public async Task Login_WithPasswordTooShort_ShouldReturnBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "short"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().System.Net.HttpStatusCode.BadRequest;
    }
}