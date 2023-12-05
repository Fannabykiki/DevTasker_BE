﻿using Capstone.Common.DTOs.Project;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.ProjectValidator
{
	public class UpdateProjectValidate : AbstractValidator<UpdateProjectNameInfo>
	{
		public UpdateProjectValidate()
		{
			RuleFor(x => x.ProjectName)
				.MaximumLength(50).WithMessage("Project name must less than 50 characters")
			.NotNull().WithMessage("Project's name is require")
			  .When(x => !string.IsNullOrWhiteSpace(x.ProjectName));
			RuleFor(x => x.Description)
			  .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
			  .When(x => !string.IsNullOrWhiteSpace(x.Description));
		}
	}
}
