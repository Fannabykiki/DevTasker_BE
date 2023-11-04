using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.Authencation
{
	public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
	{
		public ForgotPasswordValidator()
		{
			RuleFor(x => x.To).NotEmpty().WithMessage("Email is required")
				.Matches(@"^([\w\.]+)@([\w\-]+)((\.(\w){2,3})+)$")
				.WithMessage("Email format not match");
		}
	}
}
