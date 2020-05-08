using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zwyssigly.ImageServer.Contracts;

namespace Zwyssigly.ImageServer.Standalone.Images
{
    [ApiController]
    [Route("/api/v1/images/{galleryName}/{imageId}/t")]
    public class ThumbnailController : ControllerBase
    {
        private readonly IThumbnailService _thumbnailService;

        public ThumbnailController(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        [HttpGet("{id}.{ext}")]
        public async Task<IActionResult> Get([FromRoute] string galleryName, [FromRoute] string imageId, [FromRoute] string tag, [FromRoute] string ext)
        {
            var format = ImageFormat.FromExtension(ext).UnwrapOrThrow();

            var result = await _thumbnailService.GetAsync(galleryName, imageId, tag);

            return result.Match(
                thumbnail => new FileContentResult(thumbnail, format.MimeType),
                failure => failure.AsActionResult()
            );
        }
    }
}
