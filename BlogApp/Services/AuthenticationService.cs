using FirebaseAdmin.Auth;

namespace BlogApp.Services
{
    public class AuthenticationService() : IAuthenticationService
    {
        public async Task<string?> SignUp(string displayName, string email, string password)
        {
            // var userCred = await firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(email, password);
            // return userCred is null ? null : await userCred.User.GetIdTokenAsync();
            var userArgs = new UserRecordArgs(){
            Email = email,
            Password = password,
            DisplayName = displayName,
            Disabled = false
            };
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
            return userRecord?.Uid;
        }

        public async Task<string?> Login(string email, string password)
        {
            // var userCred = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);
            // return userCred is null ? null : await userCred.User.GetIdTokenAsync();
            
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            return userRecord.DisplayName;
        }

        // public void SignOut() => firebaseAuthClient.SignOut();
    }
}
