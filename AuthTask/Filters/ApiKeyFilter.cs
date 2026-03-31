using AuthTask.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthTask.Filters
{
    public class ApiKeyFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            if (
                !context.HttpContext.Request.Headers.TryGetValue(
                    "X-API-KEY",
                    out var extractedApiKey
                )
            )
            {
                context.Result = new UnauthorizedObjectResult(
                    new ApiResponse<string> { Error = "API key is missing" }
                );
                return;
            }

            var apiKey = _configuration["ApiKey"];
            if (apiKey != extractedApiKey)
            {
                context.Result = new ObjectResult(
                    new ApiResponse<string> { Error = "Invalid API key" }
                )
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                };
                return;
            }

            await next();
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : TypeFilterAttribute
    {
        public ApiKeyAttribute()
            : base(typeof(ApiKeyFilter)) { }
    }
}
