using System.ComponentModel.DataAnnotations;

namespace Execora.Application.DTOs;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public required string Email { get; set; }
}