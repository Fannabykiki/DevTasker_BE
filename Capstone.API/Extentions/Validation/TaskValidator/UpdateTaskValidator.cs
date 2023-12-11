using Capstone.Common.DTOs.Task;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.TaskValidator
{
	public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
	{
		public UpdateTaskValidator()
		{
			RuleFor(x => x.Title)
		  .NotEmpty()
		  .WithMessage("Title is required")
		  .MaximumLength(50)
		  .WithMessage("Description must be less than 50 characters");

			RuleFor(x => x.Description)
			  .MaximumLength(50000)
			  .WithMessage("Description must be less than 50.000 characters");

			RuleFor(x => x.DueDate)
			  .GreaterThanOrEqualTo(x => x.StartDate)
			  .WithMessage("Due date must be greater than or equal to start date");
		}
	}
}
