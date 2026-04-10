using AuthTask.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthTask.Filters
{
    /// <summary>
    /// Validates the request API key from the <c>X-API-KEY</c> header.
    /// </summary>
    public class ApiKeyFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyFilter"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public ApiKeyFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Executes API key validation before action execution.
        /// </summary>
        /// <param name="context">Action execution context.</param>
        /// <param name="next">Delegate to continue action execution.</param>
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

    /// <summary>
    /// Attribute wrapper for <see cref="ApiKeyFilter"/> to enable declarative API key validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyAttribute"/> class.
        /// </summary>
        public ApiKeyAttribute()
            : base(typeof(ApiKeyFilter)) { }
    }
}
