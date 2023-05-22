using System.ComponentModel.DataAnnotations;
using toons.Models.Team;
using toons.Models.User;

namespace toons.Models.Manga
{
    public class MangaChapterResponse
    {
        public int Id { get; set; }
        public UserResponse? Author { get; set; }
        public TeamResponse? Team { get; set; }
        public MangaResponse? Manga { get; set; }
        public int Volume { get; set; }
        public string Chapter { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }
}
