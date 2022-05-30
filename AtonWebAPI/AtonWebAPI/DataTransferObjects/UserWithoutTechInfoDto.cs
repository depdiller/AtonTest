using AtonWebAPI.Models;

namespace AtonWebAPI.DataTransferObjects;

public record UserWithoutTechInfoDto
{
    public string Login { get; init; }
    public string Name { get; init; }
    public Gender Gender { get; init; }
    public DateTime? Birthday { get; init; } 
    public bool Admin { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? RevokedOn { get; init; }
}