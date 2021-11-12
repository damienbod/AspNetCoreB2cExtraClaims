using Microsoft.AspNetCore.Mvc;

namespace ApiConnectorClaims.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(ILogger<ClaimsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            string content = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
            var result = new ResponseContent();
            result.Extension_CustomClaim = "everything awesome";
            return Ok(result);
        }
    }
}