using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.UserValidator
{
	public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
	{
		public ResetPasswordRequestValidator()
		{
			RuleFor(x => x.Email)
			  .NotEmpty().WithMessage("Email is required")
			  .EmailAddress().WithMessage("Email is invalid");

			RuleFor(x => x.Token)
			  .NotEmpty().WithMessage("Token is required");

			RuleFor(x => x.Password)
			  .NotEmpty().WithMessage("New password is required")
			  .MinimumLength(8)
			  .MaximumLength(16)
			  .WithMessage("Password must have at least 8 characters")
			  .Matches(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%!]).+$")
			  .WithMessage("Password must include uppercase letter, lowercase letter, number and special character");

			RuleFor(x => x.ConfirmPassword)
			  .Equal(x => x.Password).WithMessage("New password and confirm password do not match");
		}
	}
}
