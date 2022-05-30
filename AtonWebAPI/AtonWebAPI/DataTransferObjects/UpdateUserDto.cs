using System.ComponentModel.DataAnnotations;
using AtonWebAPI.Models;

namespace AtonWebAPI.DataTransferObjects;

public class UpdateUserDto
{
    [RegularExpression(@"^[a-zA-Z0-9]+$",
        ErrorMessage = "Characters are not allowed.")]
    public string? NewLogin { get; set; }
    
    [RegularExpression(@"^(([A-za-zа-яА-Я]+[\s]{1}[A-za-zа-яА-Я]+)|([A-Za-zа-яА-Я]+))$",
        ErrorMessage = "Characters are not allowed.")]
    public string? NewName { get; set;  }

    public Gender? NewGender { get; set; }
    
    public DateTime? NewBirthday { get; set; }
}