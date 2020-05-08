using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Security;
using SecurityConfiguration = Zwyssigly.ImageServer.Contracts.SecurityConfiguration;

namespace Zwyssigly.ImageServer.Standalone.Security
{
    [ApiController]
    [Route("/v1/")]
    [Authorize(PermissionTypes.Security)]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;

        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpGet("security/{galleryName}")]
        public async Task<IActionResult> Get([FromRoute]string galleryName)
        {
            var result = await _securityService.GetAsync(galleryName);
            return result.AsActionResult();
        }

        [HttpPut("security/{galleryName}")]
        public async Task<IActionResult> Put([FromRoute]string galleryName, [FromBody] SecurityConfiguration model)
        {
            var result = await _securityService.ConfigureAsync(galleryName, model);
            return result.AsActionResult();
        }

        [HttpDelete("security/{galleryName}")]
        public async Task<IActionResult> Delete([FromRoute]string galleryName)
        {
            var result = await _securityService.DeleteAsync(galleryName);
            return result.AsActionResult();
        }

        [HttpGet("security-global")]
        public async Task<IActionResult> Get()
        {
            var result = await _securityService.GetGlobalAsync();
            return result.AsActionResult();
        }

        [HttpPut("security-global")]
        public async Task<IActionResult> Put([FromBody] SecurityConfiguration model)
        {
            var result = await _securityService.ConfigureGlobalAsync(model);
            return result.AsActionResult();
        }
    }
}
