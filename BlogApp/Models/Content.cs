namespace BlogApp.Models;

public class Content
{
    public int Order { get; set; }
    public ContentTypeEnum ContentType { get; set; }
    public required string Value { get; set; }
}
