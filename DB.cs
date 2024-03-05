using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FileAPI.PostgreSQL
{
    public class FileContext : DbContext
    {
        public DbSet<Models.File> Files { get; set; }

        public FileContext(DbContextOptions<FileContext> options) : base(options) { }
    }


}