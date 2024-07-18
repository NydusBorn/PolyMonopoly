using Microsoft.AspNetCore.Mvc;

namespace asp_backend.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{

    [HttpGet]
    public bool Get1()
    {
        return true;
    }
    [HttpGet]
    public bool Get2()
    {
        return false;
    }
}