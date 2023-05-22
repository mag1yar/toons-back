using System.ComponentModel.DataAnnotations;

namespace toons.Models.Team
{
    public class TeamCreateRequest
    {
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}
