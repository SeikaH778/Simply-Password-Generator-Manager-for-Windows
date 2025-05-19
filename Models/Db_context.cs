using Microsoft.EntityFrameworkCore;
using TestApp2.Models;

namespace TestApp2
{
    class Db_context:DbContext
    {
        static string appDataPath = FileSystem.AppDataDirectory;
        static string folderPath = Path.Combine(appDataPath, "pass");
        string filePath = Path.Combine(folderPath, "passwords.db");
        public DbSet<Note> Notes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={filePath}");
    }
}
