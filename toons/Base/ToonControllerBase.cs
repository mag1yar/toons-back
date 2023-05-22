using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Extensions;
using toons.Enums;

namespace toons.Base
{
    public class ToonControllerBase : ControllerBase
    {
        [NonAction]
        protected OkObjectResult Ok(ToonStatusCode code, [ActionResultObjectValue] object? data = null, string? message = null)
        {
            return base.Ok(new
            {
                Code = code,
                StatusCode = code.GetDisplayName(),
                Message = message,
                Data = data
            });
        }

        [NonAction]
        protected BadRequestObjectResult BadRequest(ToonStatusCode code, [ActionResultObjectValue] string? message = null)
        {
            return base.BadRequest(new
            {
                Code = code,
                StatusCode = code.GetDisplayName(),
                Message = message
            });
        }
    }
}
