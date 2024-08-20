using Microsoft.AspNetCore.Mvc;

namespace UserController;

public class TryLogin : BaseUserControllerTest
{
    [Test]
    [TestCase(-1, "")]
    [TestCase(0, "")]
    public void FalseInput(int uid, string password)
    {
        _controller_.Register("test", "test");
        var result = _controller_.TryLogin(uid, password);
        Assert.That(result.GetType(), Is.EqualTo(typeof(NotFoundObjectResult)));
    }

    [Test]
    public void PartialInput()
    {
        _controller_.Register("test", "test");
        var result = _controller_.TryLogin(1, "");
        Assert.That(result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        Assert.That((bool)(result as OkObjectResult).Value, Is.EqualTo(false));
    }

    [Test]
    public void WrongInput()
    {
        _controller_.Register("test", "test");
        var result = _controller_.TryLogin(1, "nottest");
        Assert.That(result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        Assert.That((bool)(result as OkObjectResult).Value, Is.EqualTo(false));
    }

    [Test]
    public void CorrectInput()
    {
        _controller_.Register("test", "test");
        var result = _controller_.TryLogin(1, "test");
        Assert.That(result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        Assert.That((bool)(result as OkObjectResult).Value, Is.EqualTo(true));
    }
}