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
           var obj = failure.Message.Match<object>(m => new { code = failure.Code.ToString(), message = m }, () => new { code = failure.Code.ToString() });

            return failure.Code switch
            {
                Contracts.ErrorCode.NoSuchRecord => new NotFoundObjectResult(obj),
                Contracts.ErrorCode.ValidationError => new BadRequestObjectResult(obj),
                _ => new ObjectResult(obj) { StatusCode = 500 }
            };
        }
    }
}
