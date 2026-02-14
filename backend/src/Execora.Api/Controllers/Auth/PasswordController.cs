using Execora.Application.DTOs;
using Execora.Application.Services;
using Execora.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;

namespace Execora.Api.Controllers.Auth;

[ApiController]
[Route("api/auth/password")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class PasswordController : ControllerBase
{
    private readonly IPasswordResetService _passwordResetService;
    private readonly IAuditLogService _auditLogService;

    public PasswordController(
        IPasswordResetService passwordResetService,
        IAuditLogService auditLogService)
    {
        _passwordResetService = passwordResetService;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Request a password reset email
    /// </summary>
    /// <response code="200">Password reset email sent successfully</response>
    /// <response code="400">Invalid request</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            // Track IP address and user agent for security
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            // The service method is designed to not reveal if email exists
            await _passwordResetService.ForgotPasswordAsync(request);

            return Ok(new { Message = "If your email exists in our system, you will receive a password reset link shortly." });
        }
        catch (Exception ex)
        {
            await _auditLogService.LogErrorAsync(
                $"Forgot password error: {ex.Message}",
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString());

            return BadRequest(new { Error = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Reset password using a token
    /// </summary>
    /// <response code="200">Password reset successfully</response>
    /// <response code="400">Invalid request or expired token</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            // Set IP address and user agent in the request context
            // This will be captured by the service during token validation
            await _passwordResetService.ResetPasswordAsync(request);

            return Ok(new { Message = "Password has been reset successfully. Please log in with your new password." });
        }
        catch (InvalidOperationException ex)
        {
            await _auditLogService.LogErrorAsync(
                $"Password reset error: {ex.Message}",
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString());

            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            await _auditLogService.LogErrorAsync(
                $"Password reset error: {ex.Message}",
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString());

            return BadRequest(new { Error = "An error occurred while resetting your password." });
        }
    }

    /// <summary>
    /// Change user password (requires authentication)
    /// </summary>
    /// <response code="200">Password changed successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Unauthorized</response>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            // Get user ID from authenticated token
            var userId = GetAuthenticatedUserId();

            await _passwordResetService.ChangePasswordAsync(userId, request);

            return Ok(new { Message = "Password changed successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            await _auditLogService.LogErrorAsync(
                $"Unauthorized password change attempt: {ex.Message}",
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString());

            return BadRequest(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            await _auditLogService.LogErrorAsync(
                $"Password change validation error: {ex.Message}",
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString());

            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            await _auditLogService.LogErrorAsync(
                $"Password change error: {ex.Message}",
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString());

            return BadRequest(new { Error = "An error occurred while changing your password." });
        }
    }

    private Guid GetAuthenticatedUserId()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID claim not found in token");
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID format in token");
        }

        return userId;
    }

    private string GetClientIp()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Handle proxy headers (X-Forwarded-For)
        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
        }

        // Handle X-Real-IP
        else if (HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
        {
            ip = HttpContext.Request.Headers["X-Real-IP"].ToString();
        }

        return ip ?? "Unknown";
    }
}