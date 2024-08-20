using Microsoft.AspNetCore.Mvc;

namespace UserController;

public class GetUid : BaseUserControllerTest
{
    [Test]
    public void Null()
    {
        var result = _controller_.GetUid(null);
        Assert.That(result.GetType(), Is.EqualTo(typeof(NotFoundObjectResult)));
    }
    [Test]
    public void NotFound()
    {
        _controller_.Register("test", null);
        var result = _controller_.GetUid("nottest");
        Assert.That(result.GetType(), Is.EqualTo(typeof(NotFoundObjectResult)));
    }

    [Test]
    public void Found()
    {
        _controller_.Register("test", null);
        var result = _controller_.GetUid("test");
        Assert.That(result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        Assert.That((int)(result as OkObjectResult).Value, Is.EqualTo(1));
    }
}