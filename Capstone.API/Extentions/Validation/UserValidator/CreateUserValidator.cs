using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.UserValidator
{
	public class CreateUserValidator : AbstractValidator<CreateUserRequest>
	{
		public CreateUserValidator()
		{

			RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
				.Matches(@"^([\w\.]+)@([\w\-]+)((\.(\w){2,3})+)$")
				.WithMessage("Email format not match");

			RuleFor(x => x.Password)
				.NotEmpty()
				.WithMessage("Password is required")
				.MinimumLength(6)
				.MaximumLength(16)
				.WithMessage("Password must have at least 6 characters")
				.Matches(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%!]).+$")
				.WithMessage("Password must include uppercase letter, lowercase letter, number and special character");

			RuleFor(x => x).Custom((request, context) =>
			{
				if (request.Password != request.ConfirmPassword)
				{
					context.AddFailure("Confirm password is not match");
				}
			});
		}
	}
}
