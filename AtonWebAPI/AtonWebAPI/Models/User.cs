namespace AtonWebAPI.Models;

public enum Gender
{
    Male,
    Female,
    Unknown
}

public class Role
{
    public const string Admin = "Admin";
    public const string OrdinaryUser = "OrdinaryUser";
}

public record User
{
    public Guid Guid { get; init; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool Admin { get; init; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string? RevokedBy { get; set; }
}