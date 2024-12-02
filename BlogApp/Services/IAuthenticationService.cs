namespace BlogApp.Services
{
    public interface IAuthenticationService
    {
        // Task<string?> Login(string email, string password);
        // void SignOut();
        Task<string?> SignUp(string display, string email, string password);
    }
}
