using Execora.Application.DTOs;
using Execora.Application.Services;
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
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        IRegistrationService registrationService,
        ILogger<RegisterController> logger)
    {
        _registrationService = registrationService;
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
    /// <param name="userId">User ID</param>
    /// <param name="token">Verification token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpGet("{userId:guid}/verify")]
    public async Task<IActionResult> VerifyEmail(
        [FromRoute] Guid userId,
        [FromQuery] string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _registrationService.VerifyEmailAsync(userId, token, cancellationToken);

            if (success)
            {
                return Ok(new
                {
                    message = "Email verified successfully"
                });
            }

            return BadRequest(new
            {
                type = "https://example.com/probs/verification-failed",
                title = "Email verification failed",
                detail = "Invalid or expired verification token"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email verification failed for user: {UserId}", userId);
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
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPost("{userId:guid}/resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _registrationService.ResendVerificationEmailAsync(userId, cancellationToken);

            return Ok(new
            {
                message = "Verification email resent successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resend verification email for user: {UserId}", userId);
            return StatusCode(500, new
            {
                type = "https://example.com/probs/resend-verification-error",
                title = "Resend verification failed",
                detail = "An unexpected error occurred"
            });
        }
    }
}