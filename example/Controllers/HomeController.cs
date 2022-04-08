using Microsoft.AspNetCore.Mvc;

namespace example.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "/")]
    public IEnumerable<string> Get()
    {
        return HttpContext.Response.Headers.Select(h => h.ToString()).ToArray();
    }
}
