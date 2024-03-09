using FileAPI.Models.Auth;

namespace FileAPI.AuthService
{
    public interface IAuthService
    {
        public Task<User?> Login(string username, string password);
        public Task<User> Register(User user);
    }
}