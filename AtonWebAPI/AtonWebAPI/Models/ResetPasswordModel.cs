using System.ComponentModel.DataAnnotations;

namespace AtonWebAPI.Models;

public class ResetPasswordModel
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]+$",
        ErrorMessage = "Characters are not allowed.")]
    public string NewPassword { get; set; }
    
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]+$",
        ErrorMessage = "Characters are not allowed.")]
    public string ConfirmNewPassword { get; set; }
}