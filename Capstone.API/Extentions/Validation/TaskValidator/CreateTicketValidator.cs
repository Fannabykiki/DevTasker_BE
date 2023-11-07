using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.Task;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.TicketValidator
{
	public class CreateTicketValidator : AbstractValidator<CreateTaskRequest>
	{
		public CreateTicketValidator()
		{
			RuleFor(x => x.Title)
			  .NotEmpty()
			  .WithMessage("Title is required");

			RuleFor(x => x.Decription)
			  .MaximumLength(500)
			  .WithMessage("Description must be less than 500 characters");

			RuleFor(x => x.StartDate)
			  .NotEmpty()
			  .WithMessage("Start date is required");

			RuleFor(x => x.DueDate)
			  .GreaterThanOrEqualTo(x => x.StartDate)
			  .WithMessage("Due date must be greater than or equal to start date");

			RuleFor(x => x.AssignTo)
			  .NotEmpty()
			  .WithMessage("Assign to is required");

			RuleFor(x => x.PriorityId)
			  .NotEmpty()
			  .WithMessage("Priority is required");
		}
	}
}
