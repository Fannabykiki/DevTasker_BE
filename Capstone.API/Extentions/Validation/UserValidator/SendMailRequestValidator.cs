using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.UserValidator
{
	public class SendMailRequestValidator : AbstractValidator<EmailRequest>
	{
		public SendMailRequestValidator() {
			RuleFor(x => x.To).NotEmpty().WithMessage("Email is required")
					.Matches(@"^([\w\.]+)@([\w\-]+)((\.(\w){2,3})+)$")
					.WithMessage("Email format not match");
		}
	}
}
