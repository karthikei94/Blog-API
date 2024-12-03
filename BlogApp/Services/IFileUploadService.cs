namespace BlogApp.Services
{
    public interface IFileUploadService
    {
        Task<Uri> UploadFile(string name, IFormFile file);
    }
}