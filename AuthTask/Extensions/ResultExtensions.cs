using AuthTask.Application.DTOs;
using AuthTask.Application.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AuthTask.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.Status switch
            {
                ResultStatus.Success => new OkObjectResult(
                    new ApiResponse<T> { Data = result.Data }
                ),

                ResultStatus.NotFound => new NotFoundObjectResult(
                    new ApiResponse<T> { Error = result.Error }
                ),

                ResultStatus.Unauthorized => new UnauthorizedObjectResult(
                    new ApiResponse<T> { Error = result.Error }
                ),

                ResultStatus.Conflict => new ConflictObjectResult(
                    new ApiResponse<string> { Error = result.Error }
                ),

                ResultStatus.NoContent => new NoContentResult(),

                ResultStatus.Forbidden => new ObjectResult(
                    new ApiResponse<T> { Error = result.Error }
                )
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                },

                ResultStatus.Failure => new ObjectResult(
                    new ApiResponse<T> { Error = result.Error }
                )
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                },

                _ => new StatusCodeResult(500),
            };
        }
    }
}
