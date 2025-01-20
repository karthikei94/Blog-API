using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlogApp.Models;
using BlogApp.Repository;
using Google.Cloud.Storage.V1;

namespace BlogApp.Services;
public class FileUploadService : IFileUploadService
{
    private readonly BlobServiceClient _storageClient;
    private readonly IFileUploadRepository _fileUploadRepository;
    private readonly string _bucketName = "";
    // public FileUploadService(StorageClient storageClient, IConfiguration configuration, IFileUploadRepository fileUploadRepository)
    // {
    //     _storageClient = storageClient;
    //     _fileUploadRepository = fileUploadRepository;
    //     _bucketName = configuration["Firebase:StorageBucket"] ?? throw new Exception("No Bucket configuration avaiable");
    // }
    public FileUploadService(BlobServiceClient storageClient, IConfiguration configuration, IFileUploadRepository fileUploadRepository)
    {
        _storageClient = storageClient;
        _fileUploadRepository = fileUploadRepository;
        _bucketName = configuration["Firebase:StorageBucket"] ?? throw new Exception("No Bucket configuration avaiable");
    }

    public async Task<Upload> UploadFile(IFormFile file, string userId)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;
        // var blob = await _storageClient.UploadObjectAsync(_bucketName,
        // $"{file.FileName}", file.ContentType, stream);
        var containerClient = _storageClient.GetBlobContainerClient("blog-app");
        var blobClient = containerClient.GetBlobClient(file.FileName);
        Response<BlobContentInfo> result = await blobClient.ExistsAsync()
            ? await blobClient.UploadAsync(stream, true)
            : await containerClient.UploadBlobAsync(file.FileName, stream);
        // _storageClient.CreateBlobContainerAsync("blog-app",PublicAccessType.Blob)
        // var photoUrl = new Uri(containerClient.Uri.AbsoluteUri);
        Upload item = new()
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length,
            Extension = Path.GetExtension(file.FileName),
            CreatedBy = userId,
            CreatedDate = DateTime.Now,
            Url = $"{containerClient.Uri.AbsoluteUri}/{file.FileName}"
        };
        await _fileUploadRepository.AddOrUpdate(item);

        return item;
    }


}