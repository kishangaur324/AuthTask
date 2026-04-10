using AuthTask.Application.DTOs;
using FluentValidation;

namespace AuthTask.Application.Validators
{
    /// <summary>
    /// Validates register request payload.
    /// </summary>
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        /// <summary>
        /// Initializes validation rules for <see cref="RegisterRequest"/>.
        /// </summary>
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email must be a valid email address");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$")
                .WithMessage(
                    "Password must be 8-15 characters long and include at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character"
                );

            var validRoles = new[] { "admin", "employee" };

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required")
                .Must(role => validRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Role must be either 'admin' or 'employee'");
        }
    }
}
