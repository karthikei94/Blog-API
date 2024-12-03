using MongoDB.Bson.Serialization.Attributes;

namespace BlogApp.Models;
public class BlogPost
{
    [BsonId]
    public required int Id { get; set; }
    public required string Title { get; set; } = null!;
    public List<Content> Contents { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public DateTime PublicationDate { get; set; }
    public bool Draft { get; set; }
    public DateTime CreatedDate { get; set; }
    public required string CreatedBy { get; set; } = null!;
    public int Likes { get; set; }
    public int Views { get; set; }
}

public class PostComments
{
    public int Id { get; set; }
    public required int PostId {get;set;}
    public required string Value { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Likes { get; set; }
}

public class Content
{
    public int Order { get; set; }
    public ContentType ContentType { get; set; }
    public required string Value { get; set; }
}

public enum ContentType
{
    Text,
    Img
}

public class Users
{
    public int Id { get; set; }
    public required string EmailId { get; set; }
    public string PhoneNumber { get; set; }
    public required string FirstName { get; set; }
    public string LastName { get; set; }
    public required string DisplayName { get; set; }
    public bool IsAuthor { get; set; }
}

public class PagedResult<T> where T : class
{
    public long TotalRec { get; set; } = 0;
    public List<T> Records { get; set; } = [];
}