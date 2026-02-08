using Execora.Application.DTOs;
using Execora.Application.Services;
using Execora.Auth.Services;
using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Execora.Infrastructure.Services.Email;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Execora.Tests.Unit.Services;

/// <summary>
/// Unit tests for RegistrationService
/// </summary>
public class RegistrationServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITenantRepository> _mockTenantRepository;
    private readonly Mock<IAuditLogService> _mockAuditLogService;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IValidator<RegisterRequest>> _mockValidator;
    private RegistrationService _registrationService;

    public RegistrationServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTenantRepository = new Mock<ITenantRepository>();
        _mockAuditLogService = new Mock<IAuditLogService>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockEmailService = new Mock<IEmailService>();
        _mockValidator = new Mock<IValidator<RegisterRequest>>();

        // Create a test double that implements the essential functionality
        var testDbContext = new TestDbContext();

        _registrationService = new RegistrationService(
            _mockUserRepository.Object,
            _mockTenantRepository.Object,
            _mockAuditLogService.Object,
            _mockPasswordHasher.Object,
            _mockEmailService.Object,
            _mockValidator.Object,
            testDbContext
        );
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ShouldCreateUserAndTenant()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockTenantRepository.Setup(r => r.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        _mockPasswordHasher.Setup(p => p.HashPassword(request.Password))
            .Returns("hashed-password");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = "hashed-password",
            EmailConfirmed = false
        };

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.OrganizationName,
            Slug = "test-org"
        };

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockTenantRepository.Setup(r => r.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _mockUserRepository.Setup(r => r.AddTenantUserAsync(It.IsAny<TenantUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _registrationService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(tenant.Id, result.TenantId);
        Assert.Equal("TenantAdmin", result.Role);

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockTenantRepository.Verify(r => r.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _registrationService.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidPassword_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "weak",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Password", "Password does not meet requirements")
        });

        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _registrationService.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WithOrganizationSlugConflict_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var existingTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Different Org",
            Slug = "test-org"
        };

        _mockTenantRepository.Setup(r => r.GetBySlugAsync("test-org", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTenant);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _registrationService.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_ShouldLogRegistrationAuditEntry()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockTenantRepository.Setup(r => r.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        _mockPasswordHasher.Setup(p => p.HashPassword(request.Password))
            .Returns("hashed-password");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = "hashed-password"
        };

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.OrganizationName,
            Slug = "test-org"
        };

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockTenantRepository.Setup(r => r.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _mockUserRepository.Setup(r => r.AddTenantUserAsync(It.IsAny<TenantUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _registrationService.RegisterAsync(request);

        // Assert
        _mockAuditLogService.Verify(a => a.LogCreateAsync(
            It.IsAny<Guid>(),
            "User",
            user.Id,
            It.IsAny<object>(),
            user.Id,
            null,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldGenerateEmailVerificationToken()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockTenantRepository.Setup(r => r.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        _mockPasswordHasher.Setup(p => p.HashPassword(request.Password))
            .Returns("hashed-password");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = "hashed-password",
            EmailConfirmed = false
        };

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.OrganizationName,
            Slug = "test-org"
        };

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockTenantRepository.Setup(r => r.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _mockUserRepository.Setup(r => r.AddTenantUserAsync(It.IsAny<TenantUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _registrationService.RegisterAsync(request);

        // Assert
        _mockEmailService.Verify(e => e.SendEmailVerificationAsync(
            user.Email,
            It.IsAny<string>(),
            $"{user.FirstName} {user.LastName}",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Minimal DbContext implementation for testing that doesn't require complex setup
    /// </summary>
    private class TestDbContext : ExecoraDbContext
    {
        public TestDbContext() : base(new Microsoft.EntityFrameworkCore.DbContextOptions<ExecoraDbContext>())
        {
            // This is a minimal implementation that bypasses the complex IdentityDbContext inheritance
        }
    }
}