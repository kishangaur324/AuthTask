using AuthTask.Application.DTOs;
using FluentValidation;

namespace AuthTask.Application.Validators
{
    /// <summary>
    /// Validates login request payload.
    /// </summary>
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        /// <summary>
        /// Initializes validation rules for <see cref="LoginRequest"/>.
        /// </summary>
        public LoginRequestValidator()
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
        }
    }
}
