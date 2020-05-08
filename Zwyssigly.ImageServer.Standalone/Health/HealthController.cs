using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Zwyssigly.ImageServer.Standalone.Management
{
    [ApiController]
    [Route("/v1/health")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly string _version;

        public HealthController()
        {
            var assembly = Assembly.GetEntryAssembly();
            _version = assembly.GetName().Version.ToString();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(new { 
                message = "We are up and running!",
                version = _version
            });
        }
    }
}
