using Firebase.Auth;

namespace BlogApp.Services
{
    public class FirebaseAuthService(FirebaseAuthClient firebaseAuthClient) : IFirebaseAuthService
    {
        private readonly IFirebaseAuthClient firebaseAuthClient = firebaseAuthClient;

        public async Task<string?> SignUp(string email, string password)
        {
            var userCred = await firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(email, password);
            return userCred is null ? null : await userCred.User.GetIdTokenAsync();
        }

        public async Task<string?> Login(string email, string password)
        {
            var userCred = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);
            return userCred is null ? null : await userCred.User.GetIdTokenAsync();
        }

        public void SignOut() => firebaseAuthClient.SignOut();
    }
}
