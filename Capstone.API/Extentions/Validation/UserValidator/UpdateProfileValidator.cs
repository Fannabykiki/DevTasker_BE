using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.UserValidator
{
	public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
	{
        public UpdateProfileValidator()
        {
            RuleFor(x => x.UserName)
              .NotEmpty().WithMessage("User name is required");

            RuleFor(x => x.PhoneNumber)
              .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits")
              .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Gender)
              .IsInEnum().WithMessage("Invalid gender value");

            RuleFor(x => x.Status)
              .IsInEnum().WithMessage("Invalid status value");

            RuleFor(x => x.Address)
              .MaximumLength(100).WithMessage("Address must be less than 100 characters")
              .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }
}
