using Execora.Application.DTOs;
using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Execora.Infrastructure.Repositories;
using Execora.Auth.Services;
using Microsoft.Extensions.Logging;

namespace Execora.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuthenticationService> _logger;
    private const int MaxFailedLoginAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);
    private static readonly Dictionary<string, (DateTime LastFailedAttempt, int AttemptsCount)> FailedAttempts = new();

    public AuthenticationService(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IAuditLogService auditLogService,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Check if account is temporarily locked due to failed attempts
        if (IsAccountLocked(request.Email))
        {
            _logger.LogWarning("Login attempt failed - account locked for {Email}", request.Email);
            throw new AuthenticationServiceException("Account is temporarily locked. Please try again later.");
        }

        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            // Log failed attempt with non-existent user
            LogFailedAttempt(request.Email);
            _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
            throw new AuthenticationServiceException("Invalid email or password.");
        }

        // Check if account is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed - inactive account: {Email}", request.Email);
            throw new AuthenticationServiceException("Account is inactive. Please contact support.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            LogFailedAttempt(request.Email);
            _logger.LogWarning("Login failed - invalid password for: {Email}", request.Email);
            throw new AuthenticationServiceException("Invalid email or password.");
        }

        // For MVP, we'll skip email verification check
        // TODO: Uncomment when email verification is enforced
        // if (!user.EmailConfirmed)
        // {
        //     _logger.LogWarning("Login failed - email not verified: {Email}", request.Email);
        //     throw new AuthenticationServiceException("Email is not verified. Please check your inbox for the verification email.");
        // }

        // Get user's primary tenant (first one in the list)
        var primaryTenantUser = user.TenantUsers.FirstOrDefault();
        if (primaryTenantUser == null)
        {
            _logger.LogWarning("Login failed - no tenant assigned: {Email}", request.Email);
            throw new AuthenticationServiceException("User is not associated with any tenant.");
        }

        // Get user's primary tenant
        var primaryTenant = user.TenantUsers.First().Tenant;
        var primaryRole = user.TenantUsers.First().Role;

        // Generate JWT token
        var token = _tokenService.GenerateAccessToken(user, primaryTenant, primaryRole);

        // Update last login time
        await _userRepository.UpdateAsync(user);

        // Log successful login
        await _auditLogService.LogSecurityEventAsync(
            AuditAction.LoggedIn,
            "User",
            user.Id.ToString(),
            $"User {user.Email} logged in successfully",
            "127.0.0.1", // Will be set by middleware
            "Unknown" // Will be set by middleware
        );

        _logger.LogInformation("Login successful for user: {Email}", request.Email);

        // Clear failed attempts
        FailedAttempts.Remove(request.Email);

        // Return response
        return new LoginResponse
        {
            Token = token,
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailVerified,
                Role = primaryTenantUser.Role.ToString(),
                TenantId = primaryTenantUser.TenantId,
                TenantName = primaryTenantUser.Tenant.Name
            }
        };
    }

    private bool IsAccountLocked(string email)
    {
        if (!FailedAttempts.TryGetValue(email, out var attempts))
            return false;

        // Check if lockout duration has passed
        if (DateTime.UtcNow - attempts.LastFailedAttempt > LockoutDuration)
        {
            FailedAttempts.Remove(email);
            return false;
        }

        // Check if max attempts reached
        return attempts.AttemptsCount >= MaxFailedLoginAttempts;
    }

    private void LogFailedAttempt(string email)
    {
        if (!FailedAttempts.TryGetValue(email, out var attempts))
        {
            FailedAttempts[email] = (DateTime.UtcNow, 1);
        }
        else
        {
            attempts.AttemptsCount++;
            FailedAttempts[email] = (attempts.LastFailedAttempt, attempts.AttemptsCount);
        }
    }
}

// Custom exception for authentication failures
public class AuthenticationServiceException : Exception
{
    public AuthenticationServiceException(string message) : base(message)
    {
    }
}