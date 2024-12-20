using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApp.Models;
public class BlogPost
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string Title { get; set; } = null!;
    public List<Content> Contents { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public DateTime? PublicationDate { get; set; }
    public bool Draft { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public int Likes { get; set; }
    public int Views { get; set; }
}
