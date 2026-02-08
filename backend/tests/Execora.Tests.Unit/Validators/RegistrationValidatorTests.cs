using Execora.Application.Validators;
using FluentValidation.TestHelper;

namespace Execora.Tests.Unit.Validators;

/// <summary>
/// Unit tests for RegistrationValidator
/// </summary>
public class RegistrationValidatorTests
{
    private readonly RegistrationValidator _validator;

    public RegistrationValidatorTests()
    {
        _validator = new RegistrationValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid_Format()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "invalid-email",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email format is invalid");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Short1!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 12 characters long");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Uppercase()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Lowercase()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "PASSWORD123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Number()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one number");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Missing_Special_Character()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one special character");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "",
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Too_Long()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = new string('a', 101),
            LastName = "Doe",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must be less than 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "",
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Too_Long()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = new string('a', 101),
            OrganizationName = "Test Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must be less than 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_OrganizationName_Is_Empty()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = ""
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationName)
            .WithErrorMessage("Organization name is required");
    }

    [Fact]
    public void Should_Have_Error_When_OrganizationName_Is_Too_Short()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "A"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationName)
            .WithErrorMessage("Organization name must be at least 3 characters long");
    }

    [Fact]
    public void Should_Have_Error_When_OrganizationName_Is_Too_Long()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = new string('a', 101)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationName)
            .WithErrorMessage("Organization name must be less than 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_OrganizationName_Contains_Invalid_Characters()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test@Org"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationName)
            .WithErrorMessage("Organization name can only contain letters, numbers, spaces, and hyphens");
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Valid_Request()
    {
        // Arrange
        var model = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            OrganizationName = "Test Organization"
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}