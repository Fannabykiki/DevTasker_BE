using Capstone.Common.DTOs.Role;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.RoleValidator
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleRequestValidator()
        {
            RuleFor(x => x.RoleName)
              .NotEmpty().WithMessage("RoleName is required")
              .MaximumLength(100).WithMessage("RoleName must not exceed 100 characters");

            RuleFor(x => x.Description)
              .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
              .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
