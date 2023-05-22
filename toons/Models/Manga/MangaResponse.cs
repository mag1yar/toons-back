using System.ComponentModel.DataAnnotations;
using toons.Models.Team;
using toons.Models.User;

namespace toons.Models.Manga
{
    public class MangaResponse
    {
        public int Id { get; set; }
        public UserResponse? Author { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public List<TeamResponse> Teams { get; set; } = new List<TeamResponse>();
    }
}
