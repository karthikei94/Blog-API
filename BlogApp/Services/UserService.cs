using System;
using BlogApp.Models;
using Firebase.Auth;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlogApp.Services;

public class UserService : IUserService
{
    private readonly IMongoClient _mongoClient;
    private readonly IAuthenticationService authenticationService;
    private readonly IMongoCollection<Users> _collection;

    public UserService(IMongoClient mongoClient, IOptions<BlogPostsDatabaseSettings> options, IAuthenticationService authenticationService)
    {
        _mongoClient = mongoClient;
        this.authenticationService = authenticationService;
        var mongoDatabase = mongoClient.GetDatabase(options.Value.Database);
        _collection = mongoDatabase.GetCollection<Users>(Constants.CollectionNames.Users);
    }

    public async Task<Users> CreateUser(UserSignupModel model)
    {
        var uid = await authenticationService.SignUp(model.DisplayName, model.UserName, model.Password);
        if (string.IsNullOrWhiteSpace(uid))
            throw new Exception("Failed to create user on Firebase");
        var user = new Users()
        {
            UserId = uid,
            DisplayName = model.DisplayName,
            EmailId = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName
        };
        await _collection.InsertOneAsync(user);
        return user;
    }

    public async Task<Users> GetUsersAsync(string uid)
    {
        var filter = Builders<Users>.Filter.Eq("_id", uid);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Users>> GetAllUsersAsync()
    {
        var filter = Builders<Users>.Filter.Empty;
        return await _collection.Find(filter).ToListAsync();
    }
}
