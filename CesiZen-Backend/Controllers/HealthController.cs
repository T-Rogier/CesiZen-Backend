using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        public HealthController(){ }

        [HttpGet]
        public IActionResult HealthPing()
            => Ok(new { status = "OK" });
    }
}
