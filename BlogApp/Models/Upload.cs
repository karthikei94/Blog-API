using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApp.Models;
public class Upload
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string FileName { get; set; }
    public required string Extension { get; set; }
    public string ContentType { get; set; }
    public required string Url { get; set; }
    public long Size { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }

}