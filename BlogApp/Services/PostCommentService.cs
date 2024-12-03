using System.Text.Json;
using BlogApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogApp.Services
{
    public class PostCommentService : IPostCommentService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<PostComments> _collection;

        public PostCommentService(IMongoClient mongoClient, IOptions<BlogPostsDatabaseSettings> options)
        {
            _mongoClient = mongoClient;
            var mongoDatabase = mongoClient.GetDatabase(options.Value.Database);
            _collection = mongoDatabase.GetCollection<PostComments>(Constants.CollectionNames.PostComments);
        }

        public async Task AddComment(PostComments comment) =>
            await _collection.InsertOneAsync(comment);


        public async Task<List<PostComments>> GetAllAsync()
        {
            var filter = Builders<PostComments>.Filter.Empty;
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<PostComments>> GetByPostAsync(int postId)
        {
            var filter = Builders<PostComments>.Filter.Eq(d => d.PostId, postId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<PostComments> GetAsync(int id)
        {
            var filter = Builders<PostComments>.Filter.Eq(d => d.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task DeleteAsync(int id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);

        public async Task LikeComment(int id)
        {
            var filter = Builders<PostComments>.Filter.Eq(d => d.Id, id);
            var update = Builders<PostComments>.Update.Inc(d => d.Likes, 1);
            await _collection.FindOneAndUpdateAsync(filter, update);
        }

    }
}




