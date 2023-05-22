using System.ComponentModel.DataAnnotations;

namespace toons.Models.User
{
    public class UserUpdateRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(3)]
        public string Name { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}
