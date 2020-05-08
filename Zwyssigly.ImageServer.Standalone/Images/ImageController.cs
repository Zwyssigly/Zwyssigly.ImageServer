using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.Standalone.Images
{
    [ApiController]    
    [Route("/v1/images/{galleryName}")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        private async Task<byte[]> GetBodyAsByteArray()
        {
            using var memory = Request.Headers.ContentLength.HasValue
                ? new MemoryStream((int) Math.Min(int.MaxValue, Request.Headers.ContentLength.Value))
                : new MemoryStream();

            await Request.Body.CopyToAsync(memory);
            return memory.ToArray();
        }

        [HttpPost]
        [Authorize(PermissionTypes.ImageWrite)]
        public async Task<IActionResult> Upload([FromRoute] string galleryName)
        {
            var result = await _imageService.UploadAsync(galleryName, await GetBodyAsByteArray(), null);
            return result.AsActionResult();
        }

        [HttpPut("{id}")]
        [Authorize(PermissionTypes.ImageWrite)]
        public async Task<IActionResult> Replace([FromRoute] string galleryName, [FromRoute] string id)
        {
            var result = await _imageService.ReplaceAsync(galleryName, id, await GetBodyAsByteArray(), null);
            return result.AsActionResult();
        }

        [HttpGet("{id}")]
        [Authorize(PermissionTypes.ImageRead)]
        public async Task<IActionResult> Get([FromRoute] string galleryName, [FromRoute] string id)
        {
            var result = await _imageService.GetAsync(galleryName, new[] { id });
            return result.MapSuccess(i => i.Single()).AsActionResult();
        }

        [HttpGet]
        [Authorize(PermissionTypes.ImageRead)]
        public async Task<IActionResult> Get([FromRoute] string galleryName, [FromQuery] uint skip, [FromQuery] uint take)
        {
            var result = await _imageService.ListAsync(galleryName, skip, take);
            return result.AsActionResult();
        }

        [HttpDelete("{id}")]
        [Authorize(PermissionTypes.ImageWrite)]
        public async Task<IActionResult> Delete([FromRoute] string galleryName, [FromRoute] string id)
        {
            var result = await _imageService.DeleteAsync(galleryName, new[] { id });
            return result.AsActionResult();
        }
    }
}
