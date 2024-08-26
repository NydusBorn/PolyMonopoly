using asp_backend;

namespace UserController;


public class BaseUserControllerTest
{
    protected asp_backend.Controllers.UserController _controller_;
    [SetUp]
    public void Setup()
    {
        var dataDir = new DirectoryInfo("./data");
        if (!dataDir.Exists)
        {
            dataDir.Create();
        }
        Statics._userContext.Database.EnsureDeleted();
        Statics._userContext.Database.EnsureCreated();
        _controller_ = new asp_backend.Controllers.UserController();
    }
}