using System.ComponentModel.DataAnnotations;

namespace Execora.Application.DTOs;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required")]
    public required string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be between 12 and 100 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
    public required string NewPassword { get; set; }

    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }
}