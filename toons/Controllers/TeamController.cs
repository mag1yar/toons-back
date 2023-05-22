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
using toons.Models.Intermediate;
using toons.Models.Manga;
using toons.Models.Team;
using toons.Models.User;
using toons.Services.Email;
using toons.Services.File;
using toons.Services.User;
using static System.Net.Mime.MediaTypeNames;

namespace toons.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ToonControllerBase
    {
        private readonly AppDbContext _appDbContext;

        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public TeamController(AppDbContext appDbContext, IUserService userService, IFileService fileService)
        {
            _appDbContext = appDbContext;

            _userService = userService;
            _fileService = fileService;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] TeamCreateRequest request)
        {
            if (!await _appDbContext.Users.AnyAsync(u => u.Id == request.AuthorId)) 
                return BadRequest(ToonStatusCode.None, "Не найден пользователь создающий команду");
            if (await _appDbContext.Teams.AnyAsync(t => t.Name == request.Name))
                return BadRequest(ToonStatusCode.None, "Команда с этим названием уже существует");

            string? avatarName = string.Empty;
            if (request.Avatar != null)
            {
                avatarName = await _fileService.SaveImageAsync(request.Avatar);
            }

            TeamDto team = new TeamDto
            {
                AuthorId = request.AuthorId,
                Name = request.Name,
                Description = request.Description,
                AvatarName = avatarName,
                CreatedAt = DateTime.Now
            };

            await _appDbContext.Teams.AddAsync(team);
            await _appDbContext.SaveChangesAsync();

            TeamDto? createdTeam = await _appDbContext.Teams.FirstOrDefaultAsync(t => t.Name == team.Name);

            return Ok(ToonStatusCode.None, createdTeam, "Команда успешно создана");
        }

        [Authorize]
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var team = await _appDbContext.Teams
                .Include(t => t.TeamMangas)
                .ThenInclude(tm => tm.Manga)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null) return BadRequest(ToonStatusCode.None, $"Команда не с id {id} не найдена");

            UserResponse? authorResponse = await _userService.GetById(team.AuthorId);
            string? avatar = await _fileService.GetImageAsync(team.AvatarName);

            var mangas = team.TeamMangas.Select(tm =>
            {
                string? avatar = _fileService.GetImageAsync(tm.Manga.AvatarName).Result;
                UserResponse? authorResponse = _userService.GetById(tm.Manga.AuthorId).Result;

                return new MangaResponse
                {
                    Id = tm.Manga.Id,
                    Name = tm.Manga.Name,
                    Avatar = avatar,
                    Description = tm.Manga.Description,
                    Author = authorResponse
                };
            }).ToList();

            TeamResponse response = new TeamResponse {
                Id = team.Id,
                Author = authorResponse,
                Name = team.Name,
                Description = team.Description,
                Avatar = avatar,
                Mangas = mangas
            };

            return Ok(ToonStatusCode.None, response);
        }

        [Authorize]
        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            List<TeamDto> teams = await _appDbContext.Teams.ToListAsync();

            List<TeamResponse> responses = new List<TeamResponse>();
            foreach (var team in teams)
            {
                string? avatar = await _fileService.GetImageAsync(team.AvatarName);

                TeamResponse response = new TeamResponse
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    Avatar = avatar
                };

                responses.Add(response);
            }

            return Ok(ToonStatusCode.None, responses);
        }
    }
}
