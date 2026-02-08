using Execora.Application.DTOs;
using Execora.Application.Validators;
using Execora.Core.Entities;
using Execora.Core.Enums;
using Execora.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Execora.Application.Services;

/// <summary>
/// Service for handling user registration and tenant creation
/// </summary>
public class RegistrationService : IRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IValidator<RegisterRequest> _validator;

    public RegistrationService(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IAuditLogService auditLogService,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IValidator<RegisterRequest> validator)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _auditLogService = auditLogService;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _validator = validator;
    }

    /// <summary>
    /// Register a new user and create a new tenant
    /// </summary>
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException("Registration validation failed", validationResult.Errors);
        }

        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new ValidationException($"Email {request.Email} is already registered");
        }

        // Generate tenant slug and check for conflicts
        var tenantSlug = GenerateSlug(request.OrganizationName);
        var existingTenant = await _tenantRepository.GetBySlugAsync(tenantSlug, cancellationToken);
        if (existingTenant != null)
        {
            throw new ValidationException("Organization name already taken. Please choose a different name.");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = false // Email verification to be implemented in Phase 4
        };

        await _userRepository.AddAsync(user, cancellationToken);

        // Create tenant
        var tenant = new Tenant
        {
            Name = request.OrganizationName,
            Slug = tenantSlug,
            SubscriptionPlan = SubscriptionPlan.Core,
            SubscriptionStatus = SubscriptionStatus.Trial,
            SubscriptionExpiry = DateTime.UtcNow.AddDays(30) // 30-day trial
        };

        await _tenantRepository.AddAsync(tenant, cancellationToken);

        // Create tenant-user relationship with TenantAdmin role
        var tenantUser = new TenantUser
        {
            UserId = user.Id,
            TenantId = tenant.Id,
            Role = TenantRole.TenantAdmin,
            InvitedBy = user.Id,
            InvitedAt = DateTime.UtcNow,
            JoinedAt = DateTime.UtcNow
        };

        await _userRepository.AddTenantUserAsync(tenantUser, cancellationToken);

        // Log registration
        await _auditLogService.LogCreateAsync(
            tenant.Id,
            "User",
            user.Id,
            new
            {
                user.Email,
                user.FirstName,
                user.LastName,
                Tenant = new { tenant.Id, tenant.Name }
            },
            user.Id,
            null,
            null,
            null,
            cancellationToken);

        // Generate email verification token
        var emailVerificationToken = GenerateEmailVerificationToken(user.Id);

        // Send verification email (Phase 4 implementation)
        await _emailService.SendVerificationEmail(
            user.Id,
            user.Email,
            emailVerificationToken,
            null,
            cancellationToken);

        return new RegisterResponse
        {
            UserId = user.Id,
            TenantId = tenant.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            OrganizationName = tenant.Name,
            Role = tenantUser.Role.ToString(),
            EmailConfirmed = user.EmailConfirmed,
            EmailVerificationToken = emailVerificationToken
        };
    }

    /// <summary>
    /// Verify email address for a registered user
    /// </summary>
    public async Task<bool> VerifyEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        // TODO: Implement email verification token validation in Phase 4
        // For now, just mark email as confirmed
        user.EmailConfirmed = true;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Log verification
        await _auditLogService.LogStateChangeAsync(
            user.TenantUsers.FirstOrDefault()?.TenantId ?? Guid.Empty,
            "User",
            user.Id,
            "False",
            "True",
            user.Id,
            null,
            null,
            null,
            cancellationToken);

        return true;
    }

    /// <summary>
    /// Resend email verification token
    /// </summary>
    public async Task ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.EmailConfirmed)
        {
            return;
        }

        var emailVerificationToken = GenerateEmailVerificationToken(userId);

        await _emailService.SendVerificationEmail(
            userId,
            user.Email,
            emailVerificationToken,
            null,
            cancellationToken);

        // Log resend
        await _auditLogService.LogStateChangeAsync(
            user.TenantUsers.FirstOrDefault()?.TenantId ?? Guid.Empty,
            "User",
            user.Id,
            "Verification resent",
            DateTime.UtcNow.ToString(),
            user.Id,
            null,
            null,
            null,
            cancellationToken);
    }

    /// <summary>
    /// Generate URL-friendly slug from organization name
    /// </summary>
    private string GenerateSlug(string organizationName)
    {
        return organizationName
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("--", "-")
            .Replace("'", "")
            .Replace(".", "")
            .Replace(",", "")
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Aggregate(string.Empty, (current, c) => current + (c == '-' && current.EndsWith("-") ? "" : c))
            .Trim('-');
    }

    /// <summary>
    /// Generate email verification token
    /// </summary>
    private string GenerateEmailVerificationToken(Guid userId)
    {
        // TODO: Use proper token generation service in Phase 4
        // For now, generate a simple token
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}