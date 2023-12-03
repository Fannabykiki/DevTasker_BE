using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extensions.Validation.UserValidator
{
	public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
	{
		public UpdateProfileValidator()
		{
			RuleFor(x => x.Fullname)
				.MaximumLength(64).WithMessage("User name must be less than 64 characters")
				.Matches(@"^[\p{L}\s]+$").WithMessage("Invalid Fullname")
				.NotEmpty().WithMessage("User name is required");

			RuleFor(x => x.UserName)
				.MaximumLength(32).WithMessage("User name must be less than 32 characters")
				.Matches(@"^[\p{L}\p{N}]+$").WithMessage("Invalid UserName")
				.NotEmpty().WithMessage("User name is required");

			RuleFor(x => x.PhoneNumber)
				.Matches(@"^\d{9,12}$").WithMessage("Phone number must be 9-12 digits")
				.When(x => !string.IsNullOrEmpty(x.PhoneNumber));

			RuleFor(x => x.DoB)
				.Must(date => CalculateAge(date) >= 18)
				.WithMessage("Date of birth must be at least 18 years ago");

			RuleFor(x => x.Gender)
				.IsInEnum().WithMessage("Invalid gender value");

			RuleFor(x => x.Address)
				.Matches(@"^[\p{L}\p{N}\s\.,#-]+$").WithMessage("Invalid Address")
				.MaximumLength(100).WithMessage("Address must be less than 100 characters")
				.When(x => !string.IsNullOrEmpty(x.Address));

			
		}
		private int CalculateAge(DateTime? dateOfBirth)
		{
			if (dateOfBirth == null)
				return 0;
			var today = DateTime.Today;
			var age = today.Year - dateOfBirth.Value.Year;

			if (dateOfBirth.Value.Date > today.AddYears(-age))
			{
				age--;
			}
			return age;
		}
	}
}
