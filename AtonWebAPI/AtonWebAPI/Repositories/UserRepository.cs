using AtonWebAPI.DataTransferObjects;
using AtonWebAPI.Models;

namespace AtonWebAPI.Repositories;

public interface IRepository
{
    User? Get(string login);
    IDictionary<string, User> GetAll();
    void SoftDeleteUser(string login, string revokedBy);
    void HardDeleteUser(string login);
    bool Add(User user);
    bool ChangeKey(string oldKey, string newKey);
    bool Contains(string login);
    string RootName { get; }
    public UserWithoutTechInfoDto? Authenticate(string username, string password);
}

public class UserRepository : IRepository
{
    private const string _rootName = "root";

    public string RootName => _rootName;

    private readonly Dictionary<string, User> _users = new()
    {
        {
            _rootName, new User()
            {
                Guid = Guid.NewGuid(),
                Login = _rootName,
                Password = "root",
                Name = "_",
                Gender = Gender.Unknown,
                Birthday = null,
                Admin = true,
                CreatedOn = DateTime.MinValue,
                CreatedBy = null,
                ModifiedOn = null,
                ModifiedBy = null,
                RevokedOn = null
            }
        }
    };
    public IDictionary<string, User> GetAll() => _users;

    public User? Get(string login)
    {
        _users.TryGetValue(login, out var searchedUser);
        return searchedUser;
    }

    public bool Add(User newUser)
    {
        return _users.TryAdd(newUser.Login, newUser);
    }
    
    public void SoftDeleteUser(string login, string revokedBy)
    {
        _users.TryGetValue(login, out var searchedUser);
        if (searchedUser == null) return;
        searchedUser.RevokedBy = revokedBy;
        searchedUser.RevokedOn = DateTime.Now;
    }
    public void HardDeleteUser(string login)
    {
        _users.TryGetValue(login, out var searchedUser);
        if (searchedUser != null)
            _users.Remove(login);
    }

    public bool Contains(string login)
    {
        return _users.ContainsKey(login);
    }
    public UserWithoutTechInfoDto? Authenticate(string username, string password)
    {
        _users.TryGetValue(username, out var user);
        if (user == null)
            return null;
        return user.Password != password ? null : (UserWithoutTechInfoDto?) user.ToUserWithoutTechDto();
    }
    
    public bool ChangeKey(string oldKey, string newKey)
    {
        User? value;
        if (!_users.Remove(oldKey, out value))
            return false;

        _users[newKey] = value;
        return true;
    }
}