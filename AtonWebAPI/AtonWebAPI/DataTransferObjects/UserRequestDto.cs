using AtonWebAPI.Models;

namespace AtonWebAPI.DataTransferObjects;

public class UserRequestDto
{
    public string Name { get; init; }
    public Gender Gender { get; init; }
    public DateTime? Birthday { get; init; } 
    public DateTime? RevokedOn { get; init; }
}