using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AuthTask.Extensions
{
    public static class ValidationHelperExtension
    {
        public static async Task<IActionResult?> ValidateAsync<T>(
            this IValidator<T> validator,
            T instance
        )
        {
            var result = await validator.ValidateAsync(instance);

            if (result.IsValid)
                return null;

            var errors = result
                .Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            return new BadRequestObjectResult(new ValidationProblemDetails(errors));
        }
    }
}
