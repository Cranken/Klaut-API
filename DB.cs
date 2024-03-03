using Microsoft.EntityFrameworkCore;

namespace FileAPI.PostgreSQL
{
    public class FileContext : DbContext
    {
        public DbSet<File> Files { get; set; }

        public FileContext(DbContextOptions<FileContext> options) : base(options) { }
    }

    public class File
    {
        public string Id { get; set; }

        public string FileType { get; set; }
    }

}