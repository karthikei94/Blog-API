namespace BlogApp.Models;
public class UserLoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class UserSignupModel : UserLoginModel
{
    public string DisplayName { get; set; }
}