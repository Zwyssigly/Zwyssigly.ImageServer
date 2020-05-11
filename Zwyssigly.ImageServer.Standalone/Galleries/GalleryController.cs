using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.Standalone.Management
{
    [ApiController]
    [Route("/v1/galleries")]
    [Authorize(PermissionTypes.Gallery)]
    public class GalleryController : ControllerBase
    {        
        private readonly IGalleryService _service;

        public GalleryController(IGalleryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await _service.ListAsync();
            return result.AsActionResult();
        }

        [HttpDelete("{galleryName}")]
        public async Task<IActionResult> Delete([FromRoute] string galleryName)
        {
            var result = await _service.DeleteAsync(galleryName);
            return result.AsActionResult();
        }

        [HttpPost("{galleryName}")]
        public async Task<IActionResult> New([FromRoute] string galleryName)
        {
            var result = await _service.NewAsync(galleryName);
            return result.AsActionResult();
        }
    }
}
