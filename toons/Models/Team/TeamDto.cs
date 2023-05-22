using toons.Models.Intermediate;
using toons.Models.Manga;

namespace toons.Models.Team
{
    public class TeamDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? AvatarName { get; set; }
        public List<TeamManga> TeamMangas { get; set; } = new List<TeamManga>();
        public DateTime CreatedAt { get; set; }
    }
}
