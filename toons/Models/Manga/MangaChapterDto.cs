using toons.Models.Intermediate;

namespace toons.Models.Manga
{
    public class MangaChapterDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int TeamId { get; set; }
        public int MangaId { get; set; }
        public int Volume { get; set; }
        public string Chapter { get; set; } = string.Empty;
        public List<string> ImagesNames { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }
}
