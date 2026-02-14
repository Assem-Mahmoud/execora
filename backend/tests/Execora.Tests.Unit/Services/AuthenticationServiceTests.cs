using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Infrastructure.Data;
using Execora.Infrastructure.Repositories;
using Execora.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Execora.Tests.Unit.Services;

public class AuthenticationServiceTests : IDisposable
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITenantRepository> _mockTenantRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IAuditLogService> _mockAuditLogService;
    private readonly Mock<ILogger<AuthenticationService>> _mockLogger;
    private readonly Mock<ExecoraDbContext> _mockDbContext;
    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTenantRepository = new Mock<ITenantRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockAuditLogService = new Mock<IAuditLogService>();
        _mockLogger = new Mock<ILogger<AuthenticationService>>();
        _mockDbContext = new Mock<ExecoraDbContext>();

        _authenticationService = new AuthenticationService(
            _mockUserRepository.Object,
            _mockTenantRepository.Object,
            _mockTokenService.Object,
            _mockPasswordHasher.Object,
            _mockAuditLogService.Object,
            _mockLogger.Object
        );
    }

    public void Dispose()
    {
        // Cleanup if needed
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailConfirmed = true,
            FirstName = "Test",
            LastName = "User",
            PasswordHash = HashedPassword("Password123!"),
            IsActive = true,
            TenantUsers = new List<TenantUser>
            {
                new TenantUser
                {
                    TenantId = Guid.NewGuid(),
                    Role = TenantRole.TenantAdmin,
                    Tenant = new Tenant { Id = Guid.NewGuid(), Name = "Test Tenant", Slug = "test-tenant" }
                }
            }
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);

        var expectedToken = "mock-jwt-token";
        _mockTokenService
            .Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);

        // Act
        var result = await _authenticationService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.User.Id.Should().Be(user.Id);
        result.User.Email.Should().Be(user.Email);
        result.User.FirstName.Should().Be(user.FirstName);
        result.User.LastName.Should().Be(user.LastName);
        result.User.EmailConfirmed.Should().Be(user.EmailConfirmed);
        result.User.Role.Should().Be(TenantRole.TenantAdmin);
        result.User.TenantId.Should().Be(user.TenantUsers.First().TenantId);
        result.User.TenantName.Should().Be("Test Tenant");
    }

    [Fact]
    public async Task LoginAsync_WithUnconfirmedEmail_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailConfirmed = false,
            PasswordHash = HashedPassword("Password123!"),
            IsActive = true
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = () => _authenticationService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Email is not verified. Please check your inbox for the verification email.");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrong-password"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailConfirmed = true,
            PasswordHash = HashedPassword("Password123!"),
            IsActive = true
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(false);

        // Act
        Func<Task> act = () => _authenticationService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_WithInactiveAccount_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailConfirmed = true,
            PasswordHash = HashedPassword("Password123!"),
            IsActive = false
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);

        // Act
        Func<Task> act = () => _authenticationService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Account is inactive. Please contact support.");
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _authenticationService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_WithMultipleTenants_ShouldReturnFirstTenant()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            EmailConfirmed = true,
            PasswordHash = HashedPassword("Password123!"),
            IsActive = true,
            TenantUsers = new List<TenantUser>
            {
                new TenantUser
                {
                    TenantId = Guid.NewGuid(),
                    Role = TenantRole.TenantAdmin,
                    Tenant = new Tenant { Id = Guid.NewGuid(), Name = "First Tenant", Slug = "first-tenant" }
                },
                new TenantUser
                {
                    TenantId = Guid.NewGuid(),
                    Role = TenantRole.ProjectAdmin,
                    Tenant = new Tenant { Id = Guid.NewGuid(), Name = "Second Tenant", Slug = "second-tenant" }
                }
            }
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);

        var expectedToken = "mock-jwt-token";
        _mockTokenService
            .Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);

        // Act
        var result = await _authenticationService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.User.TenantId.Should().Be(user.TenantUsers.First().TenantId);
        result.User.TenantName.Should().Be("First Tenant");
        result.User.Role.Should().Be(TenantRole.TenantAdmin);
    }

    [Fact]
    public async Task LoginAsync_WithRateLimitExceeded_ShouldReturnError()
    {
        // This test will need to be implemented after the RateLimitMiddleware is created
        // and integrated with the authentication service
    }

    private static string HashedPassword(string password)
    {
        // Mock hash - in real implementation, this would use BCrypt
        return $"hashed_{password}";
    }
}