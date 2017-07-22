using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace example.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        // GET
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return HttpContext.Response.Headers.Select(h => h.ToString()).ToArray();
        }
    }
}