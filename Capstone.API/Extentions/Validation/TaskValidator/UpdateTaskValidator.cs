using Capstone.Common.DTOs.Task;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.TaskValidator
{
	public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
	{
        public UpdateTaskValidator()
        {
			RuleFor(x => x.Description)
			  .MaximumLength(500)
			  .WithMessage("Description must be less than 500 characters");

			RuleFor(x => x.DueDate)
			  .GreaterThanOrEqualTo(x => x.StartDate)
			  .WithMessage("Due date must be greater than or equal to start date");
		}
    }
}
