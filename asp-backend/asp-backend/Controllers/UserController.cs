using System.Text;
using asp_backend.Contexts;
using Microsoft.AspNetCore.Authorization;
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
    public ActionResult Register([FromQuery] string username, [FromQuery] string? password)
    {
        var possibleUser = _userContext.Users.FirstOrDefault(x => x.UserName == username);
        if (possibleUser != null)
        {
            return Conflict(possibleUser.Role == UserRole.Guest ? "Guest already exists" : "User already exists");
        }
        var user = new User
        {
            UserName = username,
            Role = password != null ? UserRole.User : UserRole.Guest,
            Created = DateTime.Now
        };
        if (password == null)
        {
            var tBuilder = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                tBuilder.Append((char)Random.Shared.Next('A', 'Z' + 1));
            }

            password = tBuilder.ToString();
        }
        user.PasswordHash = _hasher.HashPassword(user, password);
        _userContext.Users.Add(user);
        _userContext.SaveChanges();

        if (user.Role == UserRole.Guest)
        {
            Dictionary<string, string> js = new();
            js.Add("id", user.Id.ToString());
            js.Add("password", password);
            return Ok(js);
        }
        else
        {
            return Ok(user.Id);
        }
    }
    [HttpGet]
    [Authorize]
    public ActionResult<string> AuthCheck()
    {
        return Ok("I am authorised");
    }
    [HttpGet]
    [Authorize(Roles = "User")]
    public ActionResult<string> AuthCheckUser()
    {
        return Ok("I am authorised User");
    }
}