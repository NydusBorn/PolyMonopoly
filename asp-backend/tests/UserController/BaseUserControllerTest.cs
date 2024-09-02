using asp_backend;
using asp_backend.Contexts;

namespace UserController;

[NonParallelizable]
public class BaseUserControllerTest
{
    protected asp_backend.Controllers.UserController _controller_ = new ();
    [SetUp]
    public void Setup()
    {
        Statics._userContext = new UserContext();
        Statics._userContext.Database.EnsureDeleted();
        Statics._userContext.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        Statics._userContext.Database.EnsureDeleted();
    }
}