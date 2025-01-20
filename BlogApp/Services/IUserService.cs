using BlogApp.Models;

namespace BlogApp.Services;

public interface IUserService
{
    Task<Users> CreateUser(UserSignupModel model);
    Task<Users> GetUsersAsync(string uid);
    Task<List<Users>> GetAllUsersAsync();
}
