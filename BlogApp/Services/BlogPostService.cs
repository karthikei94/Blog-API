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
        public async Task<PagedResult<BlogPost>> GetPagedResultsAsync(int page, int pageSize)
        {
            var filter = Builders<BlogPost>.Filter.Empty;
            var result = new PagedResult<BlogPost>
            {
                TotalRec = await _collection.Find(filter).CountDocumentsAsync(),
                Records = await _collection.Find(filter).Skip(pageSize * (page > 1 ? page - 1 : 0)).Limit(pageSize).ToListAsync()
            };
            return result;
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

        public async Task LikePost(int id)
        {
            var filter = Builders<BlogPost>.Filter.Eq(d => d.Id, id);
            var update = Builders<BlogPost>.Update.Inc(d => d.Likes, 1);
            await _collection.FindOneAndUpdateAsync(filter, update);
        }

    }
}




