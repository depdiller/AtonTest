using System.ComponentModel.DataAnnotations;
using AtonWebAPI.Models;

namespace AtonWebAPI.DataTransferObjects;

public record CreateUserDto
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]+$",
        ErrorMessage = "Characters are not allowed.")]
    public string Login { get; init; }

    [Required]
    [RegularExpression(@"^(([A-za-zа-яА-Я]+[\s]{1}[A-za-zа-яА-Я]+)|([A-Za-zа-яА-Я]+))$",
        ErrorMessage = "Characters are not allowed.")]
    public string Name { get; init; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]+$",
        ErrorMessage = "Characters are not allowed.")]
    public string Password { get; init; }

    [Required] public Gender Gender { get; init; }
    public DateTime? Birthday { get; init; }
    [Required] public bool Admin { get; init; }
}