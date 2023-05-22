using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using System;
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
    public class MangaController : ToonControllerBase
    {
        private readonly AppDbContext _appDbContext;

        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public MangaController(AppDbContext appDbContext, IUserService userService, IFileService fileService)
        {
            _appDbContext = appDbContext;

            _userService = userService;
            _fileService = fileService;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] MangaCreateRequest request)
        {
            if (!await _appDbContext.Users.AnyAsync(u => u.Id == request.AuthorId))
                return BadRequest(ToonStatusCode.None, "Не найден пользователь создающий команду");
            if (await _appDbContext.Teams.AnyAsync(t => t.Name == request.Name))
                return BadRequest(ToonStatusCode.None, "Манга с этим названием уже существует");

            TeamDto? team = await _appDbContext.Teams.FindAsync(request.TeamId);
            if (team == null)
                return BadRequest(ToonStatusCode.None, "Не найдена команда");

            string? avatarName = string.Empty;
            if (request.Avatar != null)
            {
                avatarName = await _fileService.SaveImageAsync(request.Avatar);
            }

            MangaDto manga = new MangaDto
            {
                AuthorId = request.AuthorId,
                Name = request.Name,
                Description = request.Description,
                AvatarName = avatarName,
                CreatedAt = DateTime.Now
            };

            await _appDbContext.Mangas.AddAsync(manga);
            await _appDbContext.SaveChangesAsync();

            MangaDto? createdManga = await _appDbContext.Mangas.FirstOrDefaultAsync(t => t.Name == manga.Name);
            if (createdManga == null)
                return BadRequest(ToonStatusCode.None, "Не удалось создать мангу");

            TeamManga teamManga = new TeamManga
            {
                TeamId = team.Id,
                MangaId = createdManga.Id
            };

            _appDbContext.TeamMangas.Add(teamManga);
            await _appDbContext.SaveChangesAsync();

            return Ok(ToonStatusCode.None, createdManga, "Манга успешно создана");
        }

        [Authorize]
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            MangaDto? manga = await _appDbContext.Mangas
                .Include(t => t.TeamMangas)
                .ThenInclude(tm => tm.Team)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (manga == null) return BadRequest(ToonStatusCode.None, $"Манга не с id {id} не найдена");

            UserResponse? authorResponse = await _userService.GetById(manga.AuthorId);

            string? avatar = await _fileService.GetImageAsync(manga.AvatarName);

            var teams = manga.TeamMangas.Select(tm =>
            {
                string? avatar = _fileService.GetImageAsync(tm.Team.AvatarName).Result;
                UserResponse? authorResponse = _userService.GetById(tm.Team.AuthorId).Result;

                return new TeamResponse
                {
                    Id = tm.Team.Id,
                    Name = tm.Team.Name,
                    Avatar = avatar,
                    Description = tm.Team.Description,
                    Author = authorResponse
                };
            }).ToList();

            MangaResponse response = new MangaResponse
            {
                Id = manga.Id,
                Author = authorResponse,
                Name = manga.Name,
                Description = manga.Description,
                Avatar = avatar,
                Teams = teams
            };

            return Ok(ToonStatusCode.None, response);
        }

        [Authorize]
        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            List<MangaDto> mangaList = await _appDbContext.Mangas.ToListAsync();

            List<MangaResponse> responses = new List<MangaResponse>();
            foreach (var manga in mangaList)
            {
                string? avatar = await _fileService.GetImageAsync(manga.AvatarName);

                MangaResponse response = new MangaResponse
                {
                    Id = manga.Id,
                    Name = manga.Name,
                    Description = manga.Description,
                    Avatar = avatar
                };

                responses.Add(response);
            }

            return Ok(ToonStatusCode.None, responses);
        }

        [Authorize]
        [HttpPost("CreateChapter")]
        public async Task<IActionResult> CreateChapter([FromForm] MangaChapterCreateRequest request)
        {
            if (!await _appDbContext.Users.AnyAsync(u => u.Id == request.AuthorId))
                return BadRequest(ToonStatusCode.None, "Пользователь не найден");
            if (!await _appDbContext.Teams.AnyAsync(t => t.Id == request.TeamId))
                return BadRequest(ToonStatusCode.None, "Команда не найдена");
            if (!await _appDbContext.Mangas.AnyAsync(m => m.Id == request.MangaId))
                return BadRequest(ToonStatusCode.None, "Манга не найдена");

            List<string> ImagesNames = new List<string>();
            if (request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    ImagesNames.Add(await _fileService.SaveImageAsync(image));
                }
            }

            MangaChapterDto chapter = new MangaChapterDto
            {
                AuthorId = request.AuthorId,
                TeamId = request.TeamId,
                MangaId = request.MangaId,
                Volume = request.Volume,
                Chapter = request.Chapter,
                ImagesNames = ImagesNames,
                CreatedAt = DateTime.Now
            };

            await _appDbContext.MangaChapters.AddAsync(chapter);
            await _appDbContext.SaveChangesAsync();

            return Ok(ToonStatusCode.None, message: "Глава успешно добавлена");
        }

        [Authorize]
        [HttpGet("ChapterList")]
        public async Task<IActionResult> ChapterList([FromQuery] int mangaId, [FromQuery] int teamId)
        {
            List<MangaChapterDto> chapterList = await _appDbContext.MangaChapters.ToListAsync();

            List<MangaChapterListResponseDto> responses = new List<MangaChapterListResponseDto>();
            foreach (var chapter in chapterList)
            {
                MangaChapterListResponseDto response = new MangaChapterListResponseDto
                {
                    Id = chapter.Id,
                    AuthorId = chapter.AuthorId,
                    TeamId = chapter.TeamId,
                    MangaId = chapter.MangaId,
                    Volume = chapter.Volume,
                    Chapter = chapter.Chapter
                };

                responses.Add(response);
            }

            return Ok(ToonStatusCode.None, responses);
        }

        [Authorize]
        [HttpGet("GetChapter")]
        public async Task<IActionResult> GetChapter(int id)
        {
            MangaChapterDto? chapter = await _appDbContext.MangaChapters.FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null) return BadRequest(ToonStatusCode.None, $"Глава с id {id} не найдена");

            UserResponse? authorResponse = await _userService.GetById(chapter.AuthorId);

            List<string> images = new List<string>();
            foreach (var item in chapter.ImagesNames)
            {
                images.Add(await _fileService.GetImageAsync(item));
            }


            MangaChapterResponse response = new MangaChapterResponse
            {
                Id = chapter.Id,
                Author = authorResponse,
                Chapter = chapter.Chapter,
                Volume = chapter.Volume,
                Images = images

            };

            return Ok(ToonStatusCode.None, response);
        }
    }
}
