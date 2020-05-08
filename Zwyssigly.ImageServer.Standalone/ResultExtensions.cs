using Microsoft.AspNetCore.Mvc;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Standalone
{
    public static class ResultExtensions
    {
        public static IActionResult AsActionResult<T>(this Result<T, Contracts.Error> self)
        {
            return self.Match(
                success => success is Unit ? (IActionResult)new NoContentResult() : new OkObjectResult(success),
                failure => AsActionResult(failure));
        }

        public static IActionResult AsActionResult(this Contracts.Error failure)
        {
            return failure.Code switch
            {
                Contracts.ErrorCode.NoSuchRecord => failure.Message.Match<IActionResult>(m => new NotFoundObjectResult(m), () => new NotFoundResult()),
                Contracts.ErrorCode.ValidationError => failure.Message.Match<IActionResult>(m => new BadRequestObjectResult(failure.Message), () => new BadRequestResult()),
                _ => failure.Message.Match<IActionResult>(m => new ObjectResult(m) { StatusCode = 500 }, () => new StatusCodeResult(500))
            };
        }
    }
}
