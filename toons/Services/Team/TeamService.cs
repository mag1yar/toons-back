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
using toons.Models.Team;

namespace toons.Services.Team
{
    public class TeamService : ITeamService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;

        private readonly IFileService _fileService;

        public TeamService(IConfiguration configuration, AppDbContext appDbContext, IFileService fileService)
        {
            _configuration = configuration;
            _appDbContext = appDbContext;
            _fileService = fileService;
        }

        public Task<TeamResponse?> GetListById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
