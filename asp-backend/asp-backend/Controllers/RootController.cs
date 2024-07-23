using Microsoft.AspNetCore.Mvc;

namespace asp_backend.Controllers;
[ApiController]
[Route("[action]")]
public class RootController : Controller
{
    [HttpGet]
    public String Version()
    {
        return "PolyMonopoly InDev";
    }
}