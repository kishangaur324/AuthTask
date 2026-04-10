using AuthTask.Application.DTOs;
using FluentValidation;

namespace AuthTask.Application.Validators
{
    /// <summary>
    /// Validates pagination and search request payload.
    /// </summary>
    public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
    {
        /// <summary>
        /// Initializes validation rules for <see cref="PaginationRequest"/>.
        /// </summary>
        public PaginationRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Page number must be greater than or equal to 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Search))
                .WithMessage("Search must not exceed 100 characters");
        }
    }
}
