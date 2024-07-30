using Microsoft.AspNetCore.Mvc;

namespace asp_backend.Controllers;
[ApiController]
[Route("[action]")]
public class RootController : Controller
{
    [HttpGet]
    public String Version()
    {
        //TODO: keep updated, possibly get the version via environment/config file
        return "PolyMonopoly InDev";
    }
}