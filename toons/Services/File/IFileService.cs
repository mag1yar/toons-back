namespace toons.Services.File
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file);
        Task<string?> GetImageAsync(string? fileName);
        Task<bool> DeleteImageAsync(string fileName);
    }
}
