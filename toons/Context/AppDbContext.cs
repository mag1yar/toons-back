using Microsoft.EntityFrameworkCore;
using System.IO;
using toons.Models.Email;
using toons.Models.Intermediate;
using toons.Models.Manga;
using toons.Models.Team;
using toons.Models.User;

namespace toons.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<FileDto> Files { get; set; }
        public DbSet<UserDto> Users { get; set; }
        public DbSet<TeamDto> Teams { get; set; }

        public DbSet<MangaDto> Mangas { get; set; }
        public DbSet<MangaChapterDto> MangaChapters { get; set; }
        public DbSet<TeamManga> TeamMangas { get; set; }
    }
}
