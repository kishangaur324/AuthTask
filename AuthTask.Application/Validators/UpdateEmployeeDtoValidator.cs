using AuthTask.Application.DTOs;
using FluentValidation;

namespace AuthTask.Application.Validators
{
    /// <summary>
    /// Validates update payload for employee profile changes.
    /// </summary>
    public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
    {
        /// <summary>
        /// Initializes validation rules for <see cref="UpdateEmployeeDto"/>.
        /// </summary>
        public UpdateEmployeeDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.EmployeeCode)
                .NotEmpty()
                .WithMessage("Employee code is required")
                .MaximumLength(20)
                .WithMessage("Employee code must not exceed 20 characters");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(100)
                .WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(100)
                .WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email must be a valid email address");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .Matches(@"^\+?[0-9]{8,15}$")
                .WithMessage(
                    "Phone number must be 8 to 15 digits and may start with '+'"
                );

            RuleFor(x => x.DateOfJoining)
                .NotEmpty()
                .WithMessage("Date of joining is required")
                .Must(date => date <= DateTime.UtcNow.AddMinutes(1))
                .WithMessage("Date of joining cannot be in the future");

            RuleFor(x => x.DateOfLeaving)
                .GreaterThanOrEqualTo(x => x.DateOfJoining)
                .When(x => x.DateOfLeaving.HasValue)
                .WithMessage("Date of leaving must be greater than or equal to date of joining");

            RuleFor(x => x.DepartmentId)
                .NotEqual(Guid.Empty)
                .When(x => x.DepartmentId.HasValue)
                .WithMessage("Department id cannot be an empty GUID");

            RuleFor(x => x.ManagerId)
                .NotEqual(Guid.Empty)
                .When(x => x.ManagerId.HasValue)
                .WithMessage("Manager id cannot be an empty GUID");
        }
    }
}
