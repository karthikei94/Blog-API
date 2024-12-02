namespace BlogApp
{
    public interface IFirebaseAuthService
    {
        Task<string?> Login(string email, string password);
        void SignOut();
        Task<string?> SignUp(string email, string password);
    }
}
