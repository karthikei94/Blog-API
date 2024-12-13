using BlogApp.Services;
using Google.Cloud.Storage.V1;

public class FileUploadService : IFileUploadService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName = "";
    public FileUploadService(StorageClient storageClient, IConfiguration configuration)
    {
        _storageClient = storageClient;
        _bucketName = configuration["Firebase:StorageBucket"] ?? throw new Exception("No Bucket configuration avaiable");
    }

    public async Task<Uri> UploadFile(string name, IFormFile file)
    {

        Guid someGuid = Guid.NewGuid();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        
        var blob = await _storageClient.UploadObjectAsync(_bucketName,
        $"{name}-{someGuid}", file.ContentType, stream);

        var photoUrl = new Uri(blob.MediaLink);
        return photoUrl;
    }


}