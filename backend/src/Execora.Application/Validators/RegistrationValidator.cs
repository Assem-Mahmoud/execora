using Execora.Application.DTOs;
using FluentValidation;

namespace Execora.Application.Validators;

/// <summary>
/// Validator for RegisterRequest using FluentValidation
/// </summary>
public class RegistrationValidator : AbstractValidator<RegisterRequest>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .MaximumLength(255).WithMessage("Email must be less than 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(12).WithMessage("Password must be at least 12 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character")
            .MaximumLength(100).WithMessage("Password must be less than 100 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must be less than 100 characters")
            .Matches("^[a-zA-Z '-]+$").WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must be less than 100 characters")
            .Matches("^[a-zA-Z '-]+$").WithMessage("Last name contains invalid characters");

        RuleFor(x => x.OrganizationName)
            .NotEmpty().WithMessage("Organization name is required")
            .MinimumLength(3).WithMessage("Organization name must be at least 3 characters long")
            .MaximumLength(100).WithMessage("Organization name must be less than 100 characters")
            .Matches("^[a-zA-Z0-9 '-]+$").WithMessage("Organization name can only contain letters, numbers, spaces, and hyphens");
    }
}