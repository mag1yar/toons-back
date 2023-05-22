using toons.Models.Intermediate;

namespace toons.Models.Manga
{
    public class MangaChapterListResponseDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int TeamId { get; set; }
        public int MangaId { get; set; }
        public int Volume { get; set; }
        public string Chapter { get; set; } = string.Empty;
    }
}
