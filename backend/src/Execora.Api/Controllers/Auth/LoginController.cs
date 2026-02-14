using Execora.Application.DTOs;
using Execora.Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Execora.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IValidator<LoginRequest> _validator;

    public LoginController(
        IAuthenticationService authenticationService,
        IValidator<LoginRequest> validator)
    {
        _authenticationService = authenticationService;
        _validator = validator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        try
        {
            var result = await _authenticationService.LoginAsync(request);
            return Ok(result);
        }
        catch (System.Security.Authentication.AuthenticationException ex)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Authentication Failed",
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred during login."
            });
        }
    }
}