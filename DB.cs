using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileAPI.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace FileAPI.PostgreSQL
{
    public class FileContext : DbContext
    {
        public DbSet<Models.File> Files { get; set; }

        public FileContext(DbContextOptions<FileContext> options) : base(options) { }
    }


    public class AuthContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

    }
}