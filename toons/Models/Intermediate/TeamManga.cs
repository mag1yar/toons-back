using toons.Models.Manga;
using toons.Models.Team;

namespace toons.Models.Intermediate
{
    public class TeamManga
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public TeamDto? Team { get; set; }
        public int MangaId { get; set; }
        public MangaDto? Manga { get; set; }
    }
}
