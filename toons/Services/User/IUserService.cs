using toons.Models.User;

namespace toons.Services.User
{
    public interface IUserService
    {
        Task<UserResponse?> GetById(int id);
        string GenerateToken(UserDto user);
        string CreateRandomToken();
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
