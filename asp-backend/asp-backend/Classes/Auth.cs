using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using asp_backend.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace asp_backend;

public class Auth : AuthenticationHandler<AuthenticationSchemeOptions>
{

    public Auth(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public Auth(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(
        options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("No credentials found");
        }

        User? user;
        try
        {
            // Decode the Base64 encoded credentials from the header
            var credentialBytes = Convert.FromBase64String(Request.Headers["Authorization"]!);
            var credentialsString = Encoding.UTF8.GetString(credentialBytes);

            // Extract id
            var userId = int.Parse(credentialsString.Substring(0, credentialsString.IndexOf(':')));
            // Extract password
            var password = credentialsString.Substring(credentialsString.IndexOf(':') + 1);
            user = Statics._userContext.Users.FirstOrDefault(x => x.Id == userId);

            // Check if user is not found or invalid credentials
            if (user == null || Statics._hasher.VerifyHashedPassword(user, user.PasswordHash!, password) == PasswordVerificationResult.Failed)
            {

                return AuthenticateResult.Fail("Invalid Username or Password");
            }

        }
        catch (Exception)
        {
            return AuthenticateResult.Fail("Error Processing Authorization Header");
        }



        // Create claims based on the valid user
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        // Create an identity with claims
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        // Create principal from identity
        var principal = new ClaimsPrincipal(identity);
        // Create ticket with principal and scheme
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        // Return success with the authentication ticket
        return AuthenticateResult.Success(ticket);
    }
}