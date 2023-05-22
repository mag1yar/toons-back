using System.ComponentModel.DataAnnotations;

namespace toons.Models.User
{
    public class UserSignInRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
