using Execora.Api.Controllers.Auth;
using Execora.Application.DTOs;
using Execora.Core.Entities;
using Execora.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Execora.Tests.Integration.Controllers.Auth;

[TestClass]
public class PasswordResetTests : IntegrationTestBase
{
    private readonly HttpClient _client;
    private readonly ExecoraDbContext _context;

    public PasswordResetTests()
    {
        _client = CreateClient();
        _context = GetRequiredService<ExecoraDbContext>();
    }

    [TestMethod]
    public async Task ForgotPassword_WithValidEmail_ReturnsSuccess()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsEmailVerified = true,
            IsActive = true,
            TenantId = Guid.NewGuid(),
            PasswordHash = "hashedpassword"
        };

        var tenant = new Tenant
        {
            Id = user.TenantId,
            Name = "Test Tenant",
            Slug = "test-tenant",
            SubscriptionStatus = SubscriptionStatus.Active
        };

        await _context.Tenants.AddAsync(tenant);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var request = new { Email = "test@example.com" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/password/forgot-password", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("email address"));
    }

    [TestMethod]
    public async Task ForgotPassword_WithNonExistentEmail_ReturnsSuccess()
    {
        // Arrange
        var request = new { Email = "nonexistent@example.com" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/password/forgot-password", request);

        // Assert
        response.EnsureSuccessStatusCode();
        // Should not reveal that email doesn't exist
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("if your email exists"));
    }

    [TestMethod]
    public async Task ResetPassword_WithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            Token = "invalid-token",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/password/reset-password", request);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("Invalid or expired"));
    }

    [TestMethod]
    public async Task ChangePassword_WithInvalidCurrentPassword_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new
        {
            CurrentPassword = "WrongPassword!",
            NewPassword = "NewPassword456!",
            ConfirmPassword = "NewPassword456!"
        };

        // Add authentication header (mock)
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "fake-token");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/password/change-password", request);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task ForgotPassword_RateLimited_ReturnsTooManyRequests()
    {
        // Arrange
        var request = new { Email = "test@example.com" };

        // Make 6 requests (exceeds limit of 5 per 15 minutes)
        for (int i = 0; i < 6; i++)
        {
            await _client.PostAsJsonAsync("/api/auth/password/forgot-password", request);
        }

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/password/forgot-password", request);

        // Assert
        Assert.AreEqual(HttpStatusCode.TooManyRequests, response.StatusCode);
    }
}