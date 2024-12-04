using Microsoft.AspNetCore.Mvc;

namespace OwaspHeaders.Core.Example.Controllers;

[ApiController]
[Route("/")]
public class HomeController(ILogger<HomeController> logger) : ControllerBase
{
    private readonly ILogger<HomeController> _logger = logger;

    [HttpGet(Name = "/")]
    public IEnumerable<string> Get()
    {
        return GetHeaders;
    }

    [HttpGet("skipthis", Name = "SkipThis")]
    public IEnumerable<string> SkipThis()
    {
        return GetHeaders;
    }

    private IEnumerable<string> GetHeaders => HttpContext.Response.Headers.Select(h => h.ToString()).ToArray();
}