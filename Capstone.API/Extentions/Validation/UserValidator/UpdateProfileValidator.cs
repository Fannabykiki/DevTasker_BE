using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extensions.Validation.UserValidator
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.UserName)
                .MaximumLength(32).WithMessage("User name must be less than 32 characters")
                .NotEmpty().WithMessage("User name is required");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{9,12}$").WithMessage("Phone number must be 9-12 digits")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value");

            RuleFor(x => x.Address)
                .Matches(@"^[\p{L}\p{N}\s\.,#-]+$").WithMessage("Address is invalid")
                .MaximumLength(100).WithMessage("Address must be less than 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }
}
