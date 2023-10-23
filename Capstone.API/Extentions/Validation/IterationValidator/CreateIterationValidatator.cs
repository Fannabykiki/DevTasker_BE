using Capstone.Common.DTOs.Iteration;
using FluentValidation;

public class CreateIterationValidatator : AbstractValidator<CreateIterationRequest>
{
    public CreateIterationValidatator()
    {
        RuleFor(request => request.InterationName).NotEmpty().WithMessage("Iteration name cannot be empty");
        RuleFor(request => request.StartDate).NotEmpty().WithMessage("Start date cannot be blank");
       
        RuleFor(request => request.EndDate)
            .Must((request, endDate) => endDate > request.StartDate)
            .WithMessage("The end date must be greater than the start date");
    }
}