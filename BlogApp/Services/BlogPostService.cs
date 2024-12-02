using BlogApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogApp.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<BlogPost> _collection;

        public BlogPostService(IMongoClient mongoClient, IOptions<BlogPostsDatabaseSettings> options)
        {
            _mongoClient = mongoClient;
            var mongoDatabase = mongoClient.GetDatabase(options.Value.Database);
            _collection = mongoDatabase.GetCollection<BlogPost>(Constants.CollectionNames.BlogPost);
        }

        public async Task CreateBlogPost(BlogPost newPost) =>
            await _collection.InsertOneAsync(newPost);


        public async Task<List<BlogPost>> GetAllAsync()
        {
            var filter = Builders<BlogPost>.Filter.Empty;
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task<BlogPost> GetAsync(int id)
        {
            var filter = Builders<BlogPost>.Filter.Eq(d => d.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(int id, BlogPost post) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, post);

        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);

    }
}


