using System.ComponentModel.DataAnnotations;
using toons.Models.Manga;
using toons.Models.User;

namespace toons.Models.Team
{
    public class TeamResponse
    {
        public int Id { get; set; }
        public UserResponse? Author { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public List<MangaResponse>? Mangas { get; set; } = new List<MangaResponse>();
    }
}
