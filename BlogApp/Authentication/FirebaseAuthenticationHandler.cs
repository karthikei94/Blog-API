using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;


namespace BlogApp.Authentication;

public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public readonly FirebaseApp _firebaseApp;
    public FirebaseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        FirebaseApp firebaseApp
        ) : base(options, logger, encoder)
    {
        _firebaseApp = firebaseApp;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // this.Request.Headers.
        if (!Context.Request.Headers.ContainsKey("Authorization"))
            return Microsoft.AspNetCore.Authentication.AuthenticateResult.NoResult();

        var bearerToken = Context.Request.Headers["Authorization"];
        if (bearerToken == StringValues.Empty || !bearerToken[0].Contains("Bearer"))
            return Microsoft.AspNetCore.Authentication.AuthenticateResult.Fail("Invalid Auth Scheme");

        string token = bearerToken.ToString().Substring("Bearer ".Length);

        try
        {
            FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);

            return AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(new List<ClaimsIdentity>() {
                    new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(FirebaseAuthenticationHandler))
                }), JwtBearerDefaults.AuthenticationScheme)
            );
        }
        catch (System.Exception ex)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync("Unauthorized access");
            return AuthenticateResult.Fail(ex);
        }
        // throw new NotImplementedException();

    }

    private IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
    {

        return new List<Claim>()
        {
            new Claim("user_id",claims["user_id"].ToString()),
            new Claim("email",claims["email"].ToString()),
            new Claim("name",claims["name"].ToString()),
        };
    }
}
