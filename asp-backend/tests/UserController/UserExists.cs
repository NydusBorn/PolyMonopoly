using asp_backend;
using asp_backend.Contexts;
using Microsoft.AspNetCore.Identity;

namespace UserController;

public class UserExists : BaseUserControllerTest
{
    [Test]
    public void None()
    {
        var result = _controller_.UserExists("test");
        Assert.That(result, Is.EqualTo("None"));
    }
    [Test]
    public void Null()
    {
        var result = _controller_.UserExists(null);
        Assert.That(result, Is.EqualTo("None"));
    }
    [Test]
    public void FindGuest()
    {
        _controller_.Register("test", null);
        var result = _controller_.UserExists("test");
        Assert.That(result, Is.EqualTo("Guest"));
    }
    [Test]
    public void FindUser()
    {
        _controller_.Register("test", "test");
        var result = _controller_.UserExists("test");
        Assert.That(result, Is.EqualTo("User"));
    }
    [Test]
    public void CloakAdmin()
    {
        var user = Statics._userContext.Users.Add(new User
        {
            Created = default,
            Role = UserRole.Admin,
            UserName = "test",
        });
        user.Entity.PasswordHash = new PasswordHasher<User>().HashPassword(user.Entity, "test");
        Statics._userContext.SaveChanges();
        var result = _controller_.UserExists("test");
        Assert.That(result, Is.EqualTo("User"));
    }
}