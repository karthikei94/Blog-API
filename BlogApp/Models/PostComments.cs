namespace BlogApp.Models;

public class PostComments
{
    public int Id { get; set; }
    public required int PostId {get;set;}
    public required string Value { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Likes { get; set; }
}
