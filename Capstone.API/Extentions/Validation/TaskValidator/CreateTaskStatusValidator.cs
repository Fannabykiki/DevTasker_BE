using Capstone.Common.DTOs.Task;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.TaskValidator
{
	public class CreateTaskStatusValidator : AbstractValidator<CreateNewTaskStatus>
	{
		public CreateTaskStatusValidator()
		{
			RuleFor(x => x.Title)
			  .NotEmpty()
			  .WithMessage("Title is required");
		}
	}
}
