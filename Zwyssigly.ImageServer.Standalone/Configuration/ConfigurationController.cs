using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.Standalone.Management
{
    [ApiController]
    [Route("/v1/configurations")]
    public class ConfigurationController : ControllerBase
    {        
        private readonly IConfigurationService _service;

        public ConfigurationController(IConfigurationService service)
        {
            _service = service;
        }

        [HttpGet("{galleryName}")]
        [Authorize(PermissionTypes.ConfigurationRead)]
        public async Task<IActionResult> Get([FromRoute]string galleryName)
        {
            var result = await _service.GetAsync(galleryName);
            return result.AsActionResult();
        }

        [HttpGet]
        [Authorize(PermissionTypes.ConfigurationRead)]
        public async Task<IActionResult> List()
        {
            var result = await _service.ListAsync();
            return result.AsActionResult();
        }

        [HttpPut("{galleryName}")]
        [Authorize(PermissionTypes.ConfigurationWrite)]
        public async Task<IActionResult> Put([FromRoute]string galleryName, [FromBody] ProcessingConfiguration config)
        {
            var result = await _service.ConfigureAsync(galleryName, config);
            return result.AsActionResult();
        }

        [HttpDelete("{galleryName}")]
        [Authorize(PermissionTypes.ConfigurationWrite)]
        public async Task<IActionResult> Delete([FromRoute] string galleryName)
        {
            var result = await _service.DeleteAsync(galleryName);
            return result.AsActionResult();
        }
    }
}
