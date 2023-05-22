using System.ComponentModel.DataAnnotations;

namespace toons.Models.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
    }
}
