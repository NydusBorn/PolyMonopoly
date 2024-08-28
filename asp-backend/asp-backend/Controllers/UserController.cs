using System.ComponentModel.DataAnnotations;
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
    [HttpGet]
    public string UserExists([FromQuery, Required] string username)
    {
        var user = Statics._userContext.Users.FirstOrDefault(x => x.UserName == username);
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public ActionResult GetUid([FromQuery, Required] string username)
    {
        var uid = Statics._userContext.Users.FirstOrDefault(x => x.UserName == username)?.Id ?? -1;
        if (uid == -1)
        {
            return NotFound("User not found");
        }
        return Ok(uid);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public ActionResult TryLogin([FromQuery, Required] int uid, [FromQuery, Required] string password)
    {
        var user = Statics._userContext.Users.FirstOrDefault(x => x.Id == uid);
        if (user == null)
        {
            return NotFound("User not found");
        }
        else
        {
            if (Statics._hasher.VerifyHashedPassword(user, user.PasswordHash!, password) ==
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
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
    public ActionResult Register([FromQuery, Required] string username, [FromQuery] string? password)
    {
        if (username == "")
        {
            return BadRequest("Username cannot be empty");
        }

        if (password == "")
        {
            password = null;
        }
        var possibleUser = Statics._userContext.Users.FirstOrDefault(x => x.UserName == username);
        if (possibleUser != null)
        {
            return Conflict(possibleUser.Role == UserRole.Guest ? "Guest already exists" : "User already exists");
        }
        var user = new User
        {
            UserName = username,
            Name = username,
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
        user.PasswordHash = Statics._hasher.HashPassword(user, password);
        Statics._userContext.Users.Add(user);
        Statics._userContext.SaveChanges();

        if (user.Role == UserRole.Guest)
        {
            Dictionary<string, string> js = new()
            {
                { "id", user.Id.ToString() },
                { "password", password }
            };
            return Ok(js);
        }
        else
        {
            return Ok(user.Id);
        }
    }
}