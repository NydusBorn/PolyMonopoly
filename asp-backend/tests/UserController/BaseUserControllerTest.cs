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
        var db = new asp_backend.Contexts.UserContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        _controller_ = new asp_backend.Controllers.UserController();
    }
}