using Execora.Application.DTOs;

namespace Execora.Application.Services;

public interface IPasswordResetService
{
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
}