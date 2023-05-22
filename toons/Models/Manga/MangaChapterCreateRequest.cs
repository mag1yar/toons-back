using System.ComponentModel.DataAnnotations;

namespace toons.Models.Manga
{
    public class MangaChapterCreateRequest
    {
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int TeamId { get; set; }
        [Required]
        public int MangaId { get; set; }
        [Required]
        public int Volume { get; set; }
        [Required]
        public string Chapter { get; set; } = string.Empty;
        [Required]

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
