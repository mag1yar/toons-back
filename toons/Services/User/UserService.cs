using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using toons.Models.Email;
using MailKit.Net.Smtp;
using toons.Models.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Ocsp;
using toons.Context;
using toons.Enums;
using Microsoft.EntityFrameworkCore;
using toons.Services.File;

namespace toons.Services.User
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;

        private readonly IFileService _fileService;

        public UserService(IConfiguration configuration, AppDbContext appDbContext, IFileService fileService)
        {
            _configuration = configuration;
            _appDbContext = appDbContext;
            _fileService = fileService;
        }

        public async Task<UserResponse?> GetById(int id)
        {
            UserDto? user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            string? userAvatar = null;
            if (!string.IsNullOrEmpty(user.AvatarName))
                userAvatar = await _fileService.GetImageAsync(user.AvatarName);

            UserResponse response = new UserResponse
            {
                Id = user.Id,
                Avatar = userAvatar,
                Name = user.Name
            };

            return response;
        }

        public string GenerateToken(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                null,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);

            }
        }
    }
}
