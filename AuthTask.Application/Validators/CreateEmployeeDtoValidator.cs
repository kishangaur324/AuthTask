using AuthTask.Application.DTOs;
using FluentValidation;

namespace AuthTask.Application.Validators
{
    /// <summary>
    /// Validates create payload for employee creation.
    /// </summary>
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        /// <summary>
        /// Initializes validation rules for <see cref="CreateEmployeeDto"/>.
        /// </summary>
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email must be a valid email address");

            RuleFor(x => x.DateOfJoining)
                .NotEmpty()
                .WithMessage("Date of joining is required")
                .Must(date => date <= DateTime.UtcNow.AddMinutes(1))
                .WithMessage("Date of joining cannot be in the future");

            RuleFor(x => x.EmployeeCode)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.EmployeeCode))
                .WithMessage("Employee code must not exceed 20 characters");

            RuleFor(x => x.FirstName)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName))
                .WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.LastName))
                .WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9]{8,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage(
                    "Phone number must be 8 to 15 digits and may start with '+'"
                );
        }
    }
}
