using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiConnectorClaims.Controllers;

/// <summary>
/// Built using the following example
/// https://github.com/Azure-Samples/active-directory-dotnet-external-identities-api-connector-azure-function-validate
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ILogger<ClaimsController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public ClaimsController(ILogger<ClaimsController> logger, IConfiguration configuration, IWebHostEnvironment env)
    {
        _logger = logger;
        _configuration = configuration;
        _env = env;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Azure B2C API connector");
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync()
    {
        if (_env.IsDevelopment()) // deployment uses certificate auth
        {
            // Check HTTP basic authorization
            if (!IsAuthorizedUsingUnsecureBasicAuth(Request))
            {
                _logger.LogWarning("HTTP basic authentication validation failed.");
                return Unauthorized();
            }
        }

        string content = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
        _logger.LogInformation(content);
        var requestConnector = JsonSerializer.Deserialize<RequestConnector>(content);

        // If input data is null, show block page
        if (requestConnector == null)
        {
            return BadRequest(new ResponseContent("ShowBlockPage", "There was a problem with your request."));
        }

        string clientId = _configuration["AzureAdB2C:ClientId"];
        if (!clientId.Equals(requestConnector.ClientId))
        {
            _logger.LogWarning("HTTP clientId is not authorized.");
            return Unauthorized();
        }

        // If email claim not found, show block page. Email is required and sent by default.
        if (requestConnector.Email == null || requestConnector.Email == "" || requestConnector.Email.Contains("@") == false)
        {
            return BadRequest(new ResponseContent("ShowBlockPage", "Email name is mandatory."));
        }

        var result = new ResponseContent
        {
            // use the objectId of the email to get the user specfic claims
            MyCustomClaim = $"everything awesome {requestConnector.Email}"
        };

        return Ok(result);
    }

    private bool IsAuthorizedUsingUnsecureBasicAuth(HttpRequest req)
    {
        string username = _configuration["BasicAuthUsername"];
        string password = _configuration["BasicAuthPassword"];

        // Check if the HTTP Authorization header exist
        if (!req.Headers.ContainsKey("Authorization"))
        {
            _logger.LogWarning("Missing HTTP basic authentication header.");
            return false;
        }

        // Read the authorization header
        var auth = req.Headers["Authorization"].ToString();

        // Ensure the type of the authorization header id `Basic`
        if (!auth.StartsWith("Basic "))
        {
            _logger.LogWarning("HTTP basic authentication header must start with 'Basic '.");
            return false;
        }

        // Get the the HTTP basinc authorization credentials
        var cred = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');

        // Evaluate the credentials and return the result
        return (cred[0] == username && cred[1] == password);
    }
}