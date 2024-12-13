using BlogApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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

        public async Task CreateBlogPost(BlogPost newPost, string userId)
        {
            newPost.CreatedBy = userId;
            newPost.CreatedDate = DateTime.Now;
            await _collection.InsertOneAsync(newPost);
        }


        public async Task<List<BlogPost>> GetAllAsync()
        {
            var filter = Builders<BlogPost>.Filter.Empty;
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task<PagedResult<BlogPost>> GetPagedResultsAsync(int page, int pageSize, string loggedInUserId, bool filterCreatedBy = false)
        {
            var builder = Builders<BlogPost>.Filter;
            var filter = !filterCreatedBy ?
                            builder.Where(x => x.Draft == false && x.PublicationDate != null) :
                            builder.Empty & builder.Where(x => x.CreatedBy == loggedInUserId);

            var result = new PagedResult<BlogPost>
            {
                TotalRec = await _collection.Find(filter).CountDocumentsAsync(),
                Records = await _collection.Find(filter).Skip(pageSize * (page > 1 ? page - 1 : 0)).Limit(pageSize).ToListAsync()
            };
            return result;
        }
        public async Task<BlogPost> GetAsync(string id)
        {
            ObjectId.TryParse(id, out ObjectId objectId);
            var filter = Builders<BlogPost>.Filter.Eq("_id", objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(string id, BlogPost post)
        {
            ObjectId.TryParse(id, out ObjectId objectId);
            var filter = Builders<BlogPost>.Filter.Eq("_id", objectId);
            await _collection.ReplaceOneAsync(filter, post);
        }

        public async Task DeleteAsync(string id)
        {
            ObjectId.TryParse(id, out ObjectId objectId);
            var filter = Builders<BlogPost>.Filter.Eq("_id", objectId);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task LikePost(string id)
        {
            ObjectId.TryParse(id, out ObjectId objectId);
            var filter = Builders<BlogPost>.Filter.Eq("_id", objectId);
            var update = Builders<BlogPost>.Update.Inc(d => d.Likes, 1);
            await _collection.FindOneAndUpdateAsync(filter, update);
        }

    }
}




