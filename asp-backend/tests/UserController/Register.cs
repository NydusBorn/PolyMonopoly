using Microsoft.AspNetCore.Mvc;

namespace UserController;

public class Register : BaseUserControllerTest
{
    [Test]
    [TestCase("", "")]
    [TestCase("", "test")]
    public void FalseInput(string username, string password)
    {
        var result = _controller_.Register(username, password);
        Assert.That(result.GetType(), Is.EqualTo(typeof(BadRequestObjectResult)));
    }

    [Test]
    [TestCase("test", null)]
    [TestCase("test", "")]
    public void GuestIdentified(string username, string? password)
    {
        _controller_.Register(username, password);
        var result = _controller_.Register("test", "test");
        Assert.That(result.GetType(), Is.EqualTo(typeof(ConflictObjectResult)));
        Assert.That((string)(result as ConflictObjectResult).Value, Is.EqualTo("Guest already exists"));
    }

    [Test]
    [TestCase("test", null)]
    [TestCase("test", "")]
    public void GuestRegistered(string username, string? password)
    {
        var regResult = _controller_.Register(username, password);
        Assert.That(regResult.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        var result = _controller_.UserExists("test");
        Assert.That(result, Is.EqualTo("Guest"));
        Assert.That(((Dictionary<string,string>)(regResult as OkObjectResult).Value)["id"], Is.EqualTo("1"));
        Assert.That(((Dictionary<string,string>)(regResult as OkObjectResult).Value)["password"].Length, Is.EqualTo(16));
        var uid = int.Parse(((Dictionary<string,string>)(regResult as OkObjectResult).Value)["id"]);
        var upassword = ((Dictionary<string,string>)(regResult as OkObjectResult).Value)["password"];
        var loginResult = _controller_.TryLogin(uid, upassword);
        Assert.That(loginResult.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        Assert.That((bool)(loginResult as OkObjectResult).Value, Is.EqualTo(true));
    }

    [Test]
    public void UserIdentified()
    {
        _controller_.Register("test", "test");
        var result = _controller_.Register("test", "test");
        Assert.That(result.GetType(), Is.EqualTo(typeof(ConflictObjectResult)));
        Assert.That((string)(result as ConflictObjectResult).Value, Is.EqualTo("User already exists"));
    }

    [Test]
    public void UserRegistered()
    {
        var regResult = _controller_.Register("test", "test");
        Assert.That(regResult.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        var result = _controller_.UserExists("test");
        Assert.That(result, Is.EqualTo("User"));
        Assert.That((int)(regResult as OkObjectResult).Value, Is.EqualTo(1));
        var uid = (int)(regResult as OkObjectResult).Value;
        var loginResult = _controller_.TryLogin(uid, "test");
        Assert.That(loginResult.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        Assert.That((bool)(loginResult as OkObjectResult).Value, Is.EqualTo(true));
    }
}