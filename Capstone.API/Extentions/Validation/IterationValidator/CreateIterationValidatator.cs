using Capstone.Common.DTOs.Iteration;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.IterationValidator
{ 
    public class CreateIterationValidatator : AbstractValidator<CreateIterationRequest>
{
    public CreateIterationValidatator()
    {
        RuleFor(request => request.InterationName)
                .Matches("^[a-zA-Z0-9 ]*$").WithMessage("Iteration name should not contain special characters")
                .NotEmpty().WithMessage("Iteration name cannot be empty");
        RuleFor(request => request.StartDate).NotEmpty().WithMessage("Start date cannot be blank");

        RuleFor(request => request.EndDate)
            .Must((request, endDate) => endDate.Date > request.StartDate.Date)
            .WithMessage("The end date must be greater than the start date");
    }
}
}