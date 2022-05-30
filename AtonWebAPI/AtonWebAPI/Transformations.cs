using AtonWebAPI.Models;
using AtonWebAPI.DataTransferObjects;

namespace AtonWebAPI;

public static class Transformations
{
    public static UserWithoutTechInfoDto ToUserWithoutTechDto(this User user)
    {
        return new UserWithoutTechInfoDto
        {
            Login = user.Login,
            Name = user.Name,
            Gender = user.Gender,
            Birthday = user.Birthday,
            Admin =  user.Admin,
            CreatedOn = user.CreatedOn,
            RevokedOn = user.RevokedOn
        };
    }
    public static UserRequestDto ToUserRequestDto(this User user)
    {
        return new UserRequestDto
        {
            Name = user.Name,
            Gender = user.Gender,
            Birthday = user.Birthday,
            RevokedOn = user.RevokedOn
        };
    }
}