using BlogApp.Models;

namespace BlogApp.Services
{
    public interface IFileUploadService
    {
        Task<Upload> UploadFile(IFormFile file, string userId);
    }
}