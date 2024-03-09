using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FileAPI.Models.Auth
{
    public enum Role
    {
        Admin = 1,
        User = 2
    }
    public class User : IdentityUser
    {
        [Key]
        public override string? UserName { get; set; } = "";

        public string? Password { get; set; } = "";

        public Role Role { get; set; } = Role.User;
        public string? Token { get; set; } = "";
        public bool IsActive { get; set; } = false;

        public User(string userName, string password, Role role) : base()
        {
            UserName = userName;
            Password = password;
            Role = role;
        }
    }

    public class LoginUser
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterUser : LoginUser
    {
        public Role Role { get; set; } = Role.User;

    }
}

