namespace BlogApp.Models;

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
