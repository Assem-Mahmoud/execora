using Execora.Application.DTOs;
using Execora.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Execora.Api.Controllers.Auth;

/// <summary>
/// Controller for user registration endpoints
/// </summary>
[ApiController]
[Route("api/auth/[controller]")]
[AllowAnonymous]
public class RegisterController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        IRegistrationService registrationService,
        IEmailVerificationService emailVerificationService,
        ILogger<RegisterController> logger)
    {
        _registrationService = registrationService;
        _emailVerificationService = emailVerificationService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user and create a new tenant
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration response</returns>
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var response = await _registrationService.RegisterAsync(request, cancellationToken);

            _logger.LogInformation("Registration successful for user: {UserId}, tenant: {TenantId}",
                response.UserId, response.TenantId);

            return Ok(response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Registration validation failed for email {Email}: {Message}",
                request.Email, ex.Message);
            return BadRequest(new
            {
                type = "https://example.com/probs/validation-error",
                title = "Validation error",
                detail = ex.Message,
                errors = ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for email: {Email}", request.Email);
            return StatusCode(500, new
            {
                type = "https://example.com/probs/registration-error",
                title = "Registration failed",
                detail = "An unexpected error occurred during registration"
            });
        }
    }

    /// <summary>
    /// Verify email address
    /// </summary>
    /// <param name="request">Verification request containing token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verification response</returns>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Email verification attempt with token");

            var response = await _emailVerificationService.VerifyEmailAsync(request.Token, cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("Email verified successfully for: {Email}", response.Email);
                return Ok(response);
            }

            _logger.LogWarning("Email verification failed: {ErrorMessage}", response.ErrorMessage);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during email verification");
            return StatusCode(500, new
            {
                type = "https://example.com/probs/verification-error",
                title = "Verification error",
                detail = "An unexpected error occurred during verification"
            });
        }
    }

    /// <summary>
    /// Resend email verification token
    /// </summary>
    /// <param name="request">Resend verification request containing email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resend verification response</returns>
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail(
        [FromBody] ResendVerificationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Verification email resend attempt for: {Email}", request.Email);

            var response = await _emailVerificationService.ResendVerificationEmailAsync(request.Email, cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("Verification email resent to: {Email}", request.Email);
                return Ok(response);
            }

            _logger.LogWarning("Failed to resend verification email to: {Email}. Error: {ErrorMessage}",
                request.Email, response.ErrorMessage);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error resending verification email to: {Email}", request.Email);
            return StatusCode(500, new
            {
                type = "https://example.com/probs/resend-verification-error",
                title = "Resend verification failed",
                detail = "An unexpected error occurred"
            });
        }
    }
}