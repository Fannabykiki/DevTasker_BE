using Capstone.Common.DTOs.Project;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.ProjectValidator;

public class CreateProjectValidatator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectValidatator()
    {
        RuleFor(request => request.ProjectName).NotEmpty().WithMessage("Project name cannot be empty");
        RuleFor(request => request.StartDate).NotEmpty().WithMessage("Start date cannot be blank");
        RuleFor(request => request.Description).MaximumLength(3000).WithMessage("Description must less than 3000 characters");
        RuleFor(request => request.EndDate).NotEmpty().WithMessage("End date cannot be blank");
        RuleFor(request => request.EndDate)
            .Must((request, endDate) => endDate > request.StartDate)
            .WithMessage("The end date must be less than the start date");
    }
}