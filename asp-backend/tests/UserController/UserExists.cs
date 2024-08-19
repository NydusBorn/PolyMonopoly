using asp_backend.Contexts;
using Microsoft.AspNetCore.Identity;

namespace UserController;

public class UserExists
{
    private asp_backend.Controllers.UserController _controller_;
    [SetUp]
    public void Setup()
    {
        var dataDir = new DirectoryInfo("./data");
        if (!dataDir.Exists)
        {
            dataDir.Create();
        }
        var db = new asp_backend.Contexts.UserContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        _controller_ = new asp_backend.Controllers.UserController();
    }

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
        var db = new asp_backend.Contexts.UserContext();
        var user = db.Users.Add(new asp_backend.Contexts.User
        {
            Created = default,
            Role = UserRole.Admin,
            UserName = "test",
        });
        user.Entity.PasswordHash = new PasswordHasher<User>().HashPassword(user.Entity, "test");
        db.SaveChanges();
        var result = _controller_.UserExists("test");
        Assert.That(result, Is.EqualTo("User"));
    }
}