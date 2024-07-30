using asp_backend.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace asp_backend.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    readonly UserContext _userContext = new ();
    readonly PasswordHasher<User> _hasher = new ();
    [HttpGet]
    public string UserExists([FromQuery] string username)
    {
        var user = _userContext.Users.FirstOrDefault(x => x.UserName == username);
        if (user == null)
        {
            return "None";
        }
        else
        {
            return user.Role == UserRole.Guest ? "Guest" : "User";
        }
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(int))]
    [ProducesResponseType(404, Type = typeof(string))]
    public ActionResult GetUid([FromQuery] string username)
    {
        int uid = _userContext.Users.FirstOrDefault(x => x.UserName == username)?.Id ?? -1;
        if (uid == -1)
        {
            return NotFound("User not found");
        }
        return Ok(uid);
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(404, Type = typeof(string))]
    public ActionResult TryLogin([FromQuery] int uid, [FromQuery] string password)
    {
        var user = _userContext.Users.FirstOrDefault(x => x.Id == uid);
        if (user == null)
        {
            return NotFound("User not found");
        }
        else
        {
            if (_hasher.VerifyHashedPassword(user, user.PasswordHash!, password) ==
                PasswordVerificationResult.Success)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
    }

    [HttpPost]
    [ProducesResponseType(200, Type = typeof(int))]
    [ProducesResponseType(409, Type = typeof(string))]
    public ActionResult Register([FromQuery] string username, [FromQuery] string password)
    {
        if (_userContext.Users.Any(x => x.UserName == username))
        {
            return Conflict("User already exists");
        }
        var user = new User
        {
            UserName = username,
            Role = UserRole.User
        };
        user.PasswordHash = _hasher.HashPassword(user, password);
        _userContext.Users.Add(user);
        _userContext.SaveChanges();
        return Ok(user.Id);
    }
}