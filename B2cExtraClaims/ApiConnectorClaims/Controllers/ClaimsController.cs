using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            // TODO implement auth check
            string content = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
            var requestConnector = JsonSerializer.Deserialize<RequestConnector>(content);

            // If input data is null, show block page
            if (requestConnector == null)
            {
                return Ok(new ResponseContent("ShowBlockPage", "There was a problem with your request."));
            }

            // If email claim not found, show block page. Email is required and sent by default.
            if (requestConnector.Email == null || requestConnector.Email == "" || requestConnector.Email.Contains("@") == false)
            {
                return Ok(new ResponseContent("ShowBlockPage", "Email name is mandatory."));
            }

            // TODO verify requestConnector
            var result = new ResponseContent
            {
                // use the objectId of the email to get the user specfic claims
                MyCustomClaim = $"everything awesome {requestConnector.Email}"
            };

            return Ok(result);
        }
    }
}