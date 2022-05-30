using System.Security.Claims;
using AtonWebAPI.DataTransferObjects;
using AtonWebAPI.Models;
using AtonWebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AtonWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IRepository _repository;

    public UsersController(IRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("fullInfo")]
    [Authorize]
    public ActionResult<IEnumerable<User>> GetFullInfo()
    {
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        return Ok(_repository.GetAll().Select(userDict => userDict.Value));
    }

    [HttpGet("active")]
    [Authorize]
    public ActionResult<IEnumerable<UserWithoutTechInfoDto>> GetAllActive()
    {
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        var users = _repository.GetAll()
            .Where(userDict => userDict.Value.RevokedOn == null)
            .Select(userDict => new UserWithoutTechInfoDto
            {
                Login = userDict.Key,
                Name = userDict.Value.Name,
                Gender = userDict.Value.Gender,
                Birthday = userDict.Value.Birthday,
                Admin = userDict.Value.Admin,
                CreatedOn = userDict.Value.CreatedOn,
                RevokedOn = userDict.Value.RevokedOn
            })
            .OrderBy(user => user.CreatedOn);
        if (!users.Any())
            return NotFound();
        return Ok(users);
    }

    [HttpGet("{login}")]
    [Authorize]
    public ActionResult<UserWithoutTechInfoDto> Get(string login)
    {
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        var user = _repository.Get(login);
        if (user == null)
            return NotFound();
        return Ok(user.ToUserRequestDto());
    }

    [HttpGet("self")]
    [Authorize]
    public ActionResult<UserWithoutTechInfoDto> GetSelf()
    {
        var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = _repository.Get(currentUserLogin!);
        if (user == null)
            return NotFound();
        if (user.RevokedOn != null)
            return BadRequest("Deleted user");
        return Ok(user.ToUserWithoutTechDto());
    }

    [HttpGet("older/{age:int}")]
    [Authorize]
    public ActionResult<IEnumerable<UserRequestDto>> GetByAge(int age)
    {
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        var olderThanAge = _repository.GetAll()
            .Select(userDict => userDict.Value)
            .Where(user => user.Birthday != null && user.Birthday.Value.Age() >= age)
            .Select(user => user.ToUserRequestDto());
        return Ok(olderThanAge);
    }

    [HttpPost]
    [Authorize]
    public ActionResult CreateUser(CreateUserDto createUser)
    {
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (_repository.Contains(createUser.Login))
            return BadRequest();
        var user = new User
        {
            Guid = Guid.NewGuid(),
            Login = createUser.Login,
            Password = createUser.Password,
            Name = createUser.Name,
            Gender = createUser.Gender,
            Birthday = createUser.Birthday,
            Admin = createUser.Admin,
            CreatedOn = DateTime.Now,
            CreatedBy = currentUserLogin,
            ModifiedOn = null,
            ModifiedBy = null,
            RevokedOn = null
        };
        _repository.Add(user);
        return CreatedAtAction(nameof(CreateUser), new {id = user.Guid}, user);
    }

    [HttpDelete("delete/{login}")]
    [Authorize]
    public IActionResult SoftDelete(string login)
    {
        if (login.Equals(_repository.RootName))
            return Forbid();
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = _repository.Get(login);

        if (user is null)
            return NotFound();

        _repository.SoftDeleteUser(login, currentUserLogin);
        return Ok();
    }

    [HttpDelete("deleteCompletely/{login}")]
    [Authorize]
    public IActionResult HardDelete(string login)
    {
        if (login.Equals(_repository.RootName))
            return Forbid();
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        var user = _repository.Get(login);

        if (user is null)
            return NotFound();

        _repository.HardDeleteUser(login);
        return Ok();
    }

    [HttpPut("recover/{login}")]
    [Authorize]
    public ActionResult RecoverUser(string login)
    {
        if (!User.IsInRole(Role.Admin))
            return Forbid();
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();

        User? user = _repository.Get(login);
        if (user == null)
            return NotFound("There is no such user");
        if (user.RevokedOn == null)
            return NotFound("User cannot be restored");
        user.RevokedBy = null;
        user.RevokedOn = null;
        return Ok();
    }

    [HttpPut("resetPassword/{login}")]
    [Authorize]
    public ActionResult ResetPassword(string login,
        [FromBody] ResetPasswordModel passwordModel)
    {
        if (login.Equals(_repository.RootName))
            return Forbid();
        User? user = _repository.Get(login);
        if (user == null)
            return NotFound("There is no such user");
        if (!passwordModel.NewPassword.Equals(passwordModel.ConfirmNewPassword))
            return BadRequest("Passwords do not match");
        var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole(Role.Admin))
        {
            if (!(user.Login.Equals(currentUserLogin) && user.RevokedOn == null))
            {
                return Forbid("No access to change password");
            }
        }
        
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();
        user.Password = passwordModel.NewPassword;
        user.ModifiedBy = currentUserLogin;
        user.ModifiedOn = DateTime.Now;
        return Ok();
    }

    [HttpPut("change/{login}")]
    [Authorize]
    public ActionResult ChangeUser(string login, [FromBody] UpdateUserDto changedParameters)
    {
        if (login.Equals(_repository.RootName))
            return Forbid();
        if (changedParameters.GetType().GetProperties()
            .All(x => x.GetValue(changedParameters) == null))
            return BadRequest("There is nothing to change");

        User? user = _repository.Get(login);
        if (user == null)
            return NotFound("There is no such user");
        var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole(Role.Admin))
        {
            if (!(user.Login.Equals(currentUserLogin) && user.RevokedOn == null))
            {
                return Forbid("No access to change password");
            }
        }
        var currentUser = _repository.Get(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUser?.RevokedOn != null)
            return Forbid();

        if (changedParameters.NewLogin != null)
        {
            if (_repository.Contains(changedParameters.NewLogin))
                return BadRequest("There is already user with such login");
            user.Login = changedParameters.NewLogin;
            _repository.ChangeKey(login, changedParameters.NewLogin);
        }

        if (changedParameters.NewName != null)
            user.Name = changedParameters.NewName;

        if (changedParameters.NewBirthday != null)
            user.Birthday = changedParameters.NewBirthday;

        if (changedParameters.NewGender != null)
            user.Gender = (Gender) changedParameters.NewGender;

        user.ModifiedBy = currentUserLogin;
        user.ModifiedOn = DateTime.Now;
        return Ok();
    }
}