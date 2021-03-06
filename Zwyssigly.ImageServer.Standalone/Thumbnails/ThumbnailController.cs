﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Contracts;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.Standalone.Thumbnails
{
    [ApiController]
    [Route("/v1/thumbnails/{galleryName}/{imageId}")]
    public class ThumbnailController : ControllerBase
    {
        private readonly IThumbnailService _thumbnailService;

        public ThumbnailController(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        [HttpGet()]
        [Authorize(PermissionTypes.ThumbnailRead)]
        public async Task<IActionResult> Get([FromRoute] string galleryName, [FromRoute] string imageId, [FromQuery] string? tag, [FromQuery] ushort? minWidth, [FromQuery] ushort? minHeight)
        {
            var result = await _thumbnailService.ResolveAsync(galleryName, FromRouteArray.Parse(imageId), new ResolveOptions { Tag = tag, MinWidth = minWidth, MinHeight = minHeight });
            return result.AsActionResult();
        }

        [HttpGet("{tag}.{ext}")]
        [Authorize(PermissionTypes.ThumbnailRead)]
        public async Task<IActionResult> Get([FromRoute] string galleryName, [FromRoute] string imageId, [FromRoute] string tag, [FromRoute] string ext)
        {
            var result = await _thumbnailService.GetAsync(galleryName, imageId, tag, ext);

            return result.Match(
                thumbnail =>
                {
                    var format = ImageFormat.FromExtension(ext).UnwrapOrThrow();
                    return new FileContentResult(thumbnail, format.MimeType);
                },
                failure => failure.AsActionResult()
            );
        }

        [HttpGet("v{rowVersion}/{tag}.{ext}")]
        [Authorize(PermissionTypes.ThumbnailRead)]
        public Task<IActionResult> Get([FromRoute] string galleryName, [FromRoute] string imageId, [FromRoute] string tag, [FromRoute] uint rowVersion, [FromRoute] string ext)
        {
            // if rowVersion is specified, we can set cache to immutable
            Response.Headers.Add("cache-control", "public,max-age=31536000,immutable");

            return Get(galleryName, imageId, tag, ext);
        }
    }
}
