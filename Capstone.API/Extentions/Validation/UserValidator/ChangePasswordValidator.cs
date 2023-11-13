using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.UserValidator
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator() {
            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Email is required")
              .EmailAddress().WithMessage("Email is invalid");

            RuleFor(x => x.Token)
              .NotEmpty().WithMessage("Token is required");

            RuleFor(x => x.CurrentPassword)
              .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
              .NotEmpty().WithMessage("New password is required")
              .MinimumLength(6)
              .MaximumLength(16)
              .WithMessage("Password must have at least 6 characters")
              .Matches(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%!]).+$")
              .WithMessage("Password must include uppercase letter, lowercase letter, number and special character");

            RuleFor(x => x.ConfirmPassword)
              .Equal(x => x.NewPassword).WithMessage("New password and confirm password do not match");
        }
    }
}
