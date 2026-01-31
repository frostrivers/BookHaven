using Microsoft.AspNetCore.Mvc;

namespace BookHaven.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController
    {
        [HttpGet("")]
        public string Version(CancellationToken cancellationToken)
        {
            return "Version 1.0";
        }
    }
}
