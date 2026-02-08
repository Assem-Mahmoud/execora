using Execora.Application.DTOs;
using Execora.Core.Enums;

namespace Execora.Application.Services;

/// <summary>
/// Service for handling user registration and tenant creation
/// </summary>
public interface IRegistrationService
{
    /// <summary>
    /// Register a new user and create a new tenant
    /// </summary>
    /// <param name="request">Registration request containing user and tenant details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration response with user and tenant information</returns>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify email address for a registered user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="token">Email verification token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if verification was successful</returns>
    Task<bool> VerifyEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resend email verification token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken = default);
}