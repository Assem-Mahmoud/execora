using Execora.Application.DTOs;

namespace Execora.Application.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}