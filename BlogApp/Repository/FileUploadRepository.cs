using BlogApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogApp.Repository;

public interface IFileUploadRepository
{
    Task<Upload> AddOrUpdate(Upload upload);
}
public class FileUploadRepository : IFileUploadRepository
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoCollection<Upload> _collection;
    public FileUploadRepository(IMongoClient mongoClient, IOptions<BlogPostsDatabaseSettings> options)
    {
        _mongoClient = mongoClient;
        var mongoDatabase = _mongoClient.GetDatabase(options.Value.Database);
        _collection = mongoDatabase.GetCollection<Upload>(Constants.CollectionNames.Upload);
    }

    public async Task<Upload> AddOrUpdate(Upload upload)
    {
        var filter = Builders<Upload>.Filter.Where(x => x.FileName == upload.FileName);
        var exists = await _collection.Find(filter).FirstOrDefaultAsync();
        // _collection.UpdateOne(filter,)
        if(exists is not null){
            upload.Id = exists.Id;
            _ = await _collection.ReplaceOneAsync(filter, upload);
        }
        else
            await _collection.InsertOneAsync(upload);
        return upload;
    }
}