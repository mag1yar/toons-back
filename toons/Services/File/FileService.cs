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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using toons.Context;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Net.Mime;

namespace toons.Services.File
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _environment;

        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" };

        public FileService(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _appDbContext = appDbContext;
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !_permittedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file extension.");

            var targetFolder = Path.Combine(_environment.WebRootPath, "Images");

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            FileDto newFile = new FileDto { Name = file.FileName, Path = "Images" };

            await _appDbContext.Files.AddAsync(newFile);
            await _appDbContext.SaveChangesAsync();

            return uniqueFileName;
        }

        public async Task<string?> GetImageAsync(string? fileName)
        {
            if(string.IsNullOrEmpty(fileName)) return null;

            var targetFolder = Path.Combine(_environment.WebRootPath, "Images");
            var filePath = Path.Combine(targetFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return null;

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileExtension = Path.GetExtension(filePath).TrimStart('.');
            var contentType = $"image/{fileExtension}";
            var base64String = Convert.ToBase64String(fileBytes);

            return $"data:{contentType};base64,{base64String}";
        }

        public async Task<bool> DeleteImageAsync(string fileName)
        {
            var targetFolder = Path.Combine(_environment.WebRootPath, "Images");
            var filePath = Path.Combine(targetFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return false;

            System.IO.File.Delete(filePath);

            var file = _appDbContext.Files.FirstOrDefault(f => f.Name == fileName);

            if (file != null)
            {
                _appDbContext.Files.Remove(file);
                await _appDbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}
