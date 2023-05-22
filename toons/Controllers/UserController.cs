using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using toons.Base;
using toons.Context;
using toons.Enums;
using toons.Models.Email;
using toons.Models.User;
using toons.Services.Email;
using toons.Services.File;
using toons.Services.User;
using static System.Net.Mime.MediaTypeNames;

namespace toons.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ToonControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;

        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;

        public UserController(IConfiguration configuration, AppDbContext appDbContext, IUserService userService, IEmailService emailService, IFileService fileService)
        {
            _configuration = configuration;
            _appDbContext = appDbContext;

            _userService = userService;
            _emailService = emailService;
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] UserSignInRequest request)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return BadRequest(ToonStatusCode.AuthError, "Пошта немесе құпия сөз дұрыс емес");
            if (!_userService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest(ToonStatusCode.AuthError, "Пошта немесе құпия сөз дұрыс емес");
            if (user.VerifiedAt == null) return BadRequest(ToonStatusCode.AuthError, "Пайдаланушы расталған жоқ, поштаңызды тексеріңіз");

            string? userAvatar = await _fileService.GetImageAsync(user.AvatarName);

            var accessToken = _userService.GenerateToken(user);

            UserResponse response = new UserResponse
            {
                AccessToken = accessToken,
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Avatar = userAvatar
            };

            return Ok(ToonStatusCode.AuthSignIn, response, "Сәтті авторизацияландыңыз");
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpRequest request)
        {
            try
            {
                if (await _appDbContext.Users.AnyAsync(u => u.Email == request.Email))
                    return BadRequest(ToonStatusCode.AuthError, "Бұл пошта басқа пайдаланушы тарапынан пайдаланылады.");
                if (await _appDbContext.Users.AnyAsync(u => u.Name == request.Name))
                    return BadRequest(ToonStatusCode.AuthError, "Бұл логин басқа пайдаланушы тарапынан пайдаланылады.");

                _userService.CreatePasswordHash(request.Password, out byte[] PasswordHash, out byte[] PasswordSalt);

                UserDto user = new UserDto
                {
                    Email = request.Email,
                    Name = request.Name,
                    PasswordHash = PasswordHash,
                    PasswordSalt = PasswordSalt,
                    VerificationToken = _userService.CreateRandomToken(),
                    CreatedAt = DateTime.Now
                };

                await _appDbContext.Users.AddAsync(user);

                EmailDto email = new EmailDto
                {
                    To = user.Email,
                    Subject = "Toons.kz сайтындағы тіркелгіні растау",
                    Body = $"Тіркелгіні растау үшін <a href=\"{_configuration["ProductionAddress"]}/confirm?t={user.VerificationToken}&m=1\">мұнда</a> басыңыз"
                };


                bool send = _emailService.SendEmail(email);

                if (!send) return BadRequest(ToonStatusCode.EmailError, "Тіркелгіні растау сілтемесін жіберу мүмкін болмады");

                await _appDbContext.SaveChangesAsync();

                return Ok(ToonStatusCode.AuthSignUp, message: "Пайдаланушы сәтті жасалды");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating user: " + ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null) return BadRequest(ToonStatusCode.AuthError, "Дұрыс емес токен");
            if (user.VerifiedAt.HasValue) return BadRequest(ToonStatusCode.AuthError, "Пайдаланушы алдақашан расталған");

            user.VerifiedAt = DateTime.Now;
            await _appDbContext.SaveChangesAsync();

            return Ok(ToonStatusCode.None, message: "Вы успешно подтвердили аккаунт");
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return BadRequest(ToonStatusCode.AuthError, "Мұндай поштасы бар пайдаланушыны табу мүмкін болмады");

            user.PasswordResetToken = _userService.CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);

            EmailDto _email = new EmailDto
            {
                To = user.Email,
                Subject = "Toons.kz сайттағы құпия сөзді қалпына келтіріңіз",
                Body = $"Құпия сөзді қайтару үшін <a href=\"{_configuration["ProductionAddress"]}/confirm?t={user.PasswordResetToken}&m=2\">мұнда</a> басыңыз"
            };

            bool send = _emailService.SendEmail(_email);
            if (!send) return BadRequest(ToonStatusCode.EmailError, "Не удалось отправить письмо на почту с ссылкой для того чтобы сбросить пароль");

            await _appDbContext.SaveChangesAsync();

            return Ok(ToonStatusCode.None, message: "Құпия сөзді қайтару үшін сілтеме поштаңызға жіберілді");
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordRequest request)
        {
            UserDto? user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);

            if (user == null || user.ResetTokenExpires < DateTime.Now) return BadRequest(ToonStatusCode.AuthError, "Invalid token.");

            _userService.CreatePasswordHash(request.Password, out byte[] PasswordHash, out byte[] PasswordSalt);

            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PasswordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _appDbContext.SaveChangesAsync();

            return Ok(ToonStatusCode.None, message: "Құпия сөз сәтті өзгертілді.");
        }

        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateRequest request)
        {
            UserDto? user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);

            if (user == null) return BadRequest(ToonStatusCode.AuthError, "Пользователь не найден.");

            if (request.Avatar != null)
            {
                string? oldAvatarName = user.AvatarName;
                user.AvatarName = await _fileService.SaveImageAsync(request.Avatar);
                if (user.AvatarName != null && !string.IsNullOrEmpty(oldAvatarName))
                {
                    await _fileService.DeleteImageAsync(oldAvatarName);
                }
            }

            await _appDbContext.SaveChangesAsync();

            string? userAvatar = await _fileService.GetImageAsync(user.AvatarName);

            var accessToken = _userService.GenerateToken(user);

            UserResponse response = new UserResponse
            {
                AccessToken = accessToken,
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Avatar = userAvatar
            };

            return Ok(ToonStatusCode.AuthSignIn, response, "Данные пользователя успешно изменены.");
        }

        [Authorize]
        [HttpGet("ValidateToken")]
        public IActionResult ValidateToken()
        {
            return Ok(ToonStatusCode.None, true, "Token is valid!");
        }
    }
}
