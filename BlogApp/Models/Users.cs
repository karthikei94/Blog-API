using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApp.Models;

public class Users
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string UserId {get;set;}
    public required string EmailId { get; set; }
    public string PhoneNumber { get; set; }
    public required string FirstName { get; set; }
    public string LastName { get; set; }
    public required string DisplayName { get; set; }
    public bool IsAuthor { get; set; }
}
