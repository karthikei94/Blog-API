using System.Text.Json;
using BlogApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
            var mongoDatabase = _mongoClient.GetDatabase(options.Value.Database);
            _collection = mongoDatabase.GetCollection<PostComments>(Constants.CollectionNames.PostComments);
        }

        public async Task<PostComments> AddComment(PostComments comment)
        {
            // comment.Id = ObjectId.Empty;
            await _collection.InsertOneAsync(comment);
            return comment;
        }


        public async Task<List<PostComments>> GetAllAsync()
        {
            var filter = Builders<PostComments>.Filter.Empty;
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<PostComments>> GetByPostAsync(string postId)
        {
            // ObjectId.TryParse(postId, out ObjectId objectId);
            var filter = Builders<PostComments>.Filter.Where(x => x.PostId == postId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<PostComments> GetAsync(string id)
        {
            ObjectId.TryParse(id, out ObjectId objectId);
            var filter = Builders<PostComments>.Filter.Eq("_id", objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task DeleteAsync(string id)
        {
            ObjectId.TryParse(id, out ObjectId objectId);
            var filter = Builders<PostComments>.Filter.Eq("_id", objectId);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task LikeComment(string id)
        {
            ObjectId.TryParse(id, out ObjectId objectId);
            var filter = Builders<PostComments>.Filter.Eq("_id", objectId);
            var update = Builders<PostComments>.Update.Inc(d => d.Likes, 1);
            await _collection.FindOneAndUpdateAsync(filter, update);
        }

    }
}




