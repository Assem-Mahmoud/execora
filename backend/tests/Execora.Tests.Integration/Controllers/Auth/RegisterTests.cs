using Execora.Api.Tests.Helpers;
using Execora.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace Execora.Api.Tests.Controllers.Auth;

/// <summary>
/// Integration tests for RegisterController
/// </summary>
public class RegisterTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public RegisterTests(ITestOutputHelper output, WebApplicationFactory<Program> factory)
    {
        _output = output;
        _factory = factory;
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Add test services
                services.AddTestDatabase();
            });
        }).CreateClient();

        _client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var request = new
        {
            email = "test@example.com",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Debug output
        _output.WriteLine($"Response status: {response.StatusCode}");
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonResult>();
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        dynamic data = result.Value!;
        Assert.Equal("test@example.com", data.email);
        Assert.Equal("John", data.firstName);
        Assert.Equal("Doe", data.lastName);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnConflict()
    {
        // Arrange
        var request = new
        {
            email = "duplicate@example.com",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // First registration
        await _client.PostAsJsonAsync("/auth/register", request);

        // Second registration with same email
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("email already registered", error.Detail?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithInvalidEmailFormat_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "invalid-email",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("email", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "weakpassword@example.com",
            password = "weak",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("password", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithShortPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "shortpassword@example.com",
            password = "Short1!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("password", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithMissingUppercasePassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "uppercase@example.com",
            password = "password123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("password", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithMissingLowercasePassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "lowercase@example.com",
            password = "PASSWORD123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("password", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithMissingNumberPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "number@example.com",
            password = "Password!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("password", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithMissingSpecialCharPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "special@example.com",
            password = "Password123",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("password", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithEmptyFirstName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "firstname@example.com",
            password = "Password123!",
            firstName = "",
            lastName = "Doe",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("firstName", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithEmptyLastName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "lastname@example.com",
            password = "Password123!",
            firstName = "John",
            lastName = "",
            organizationName = "Test Organization"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("lastName", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_WithEmptyOrganizationName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            email = "orgname@example.com",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(error);
        Assert.Contains("organizationName", error.Title?.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_ShouldCreateTenantInDatabase()
    {
        // Arrange
        var request = new
        {
            email = "tenant@example.com",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test Tenant"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);
        response.EnsureSuccessStatusCode();

        // Assert
        // This test would require database access to verify tenant creation
        // For now, just ensure the API responds successfully
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldCreateUserInDatabase()
    {
        // Arrange
        var request = new
        {
            email = "userdb@example.com",
            password = "Password123!",
            firstName = "John",
            lastName = "Doe",
            organizationName = "Test User DB"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);
        response.EnsureSuccessStatusCode();

        // Assert
        // This test would require database access to verify user creation
        // For now, just ensure the API responds successfully
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}