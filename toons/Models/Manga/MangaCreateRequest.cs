using System.ComponentModel.DataAnnotations;

namespace toons.Models.Manga
{
    public class MangaCreateRequest
    {
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int TeamId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}
