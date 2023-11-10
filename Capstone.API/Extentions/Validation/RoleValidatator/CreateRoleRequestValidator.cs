using Capstone.Common.DTOs.Role;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.RoleValidator
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateNewRoleRequest>
    {
        public CreateRoleRequestValidator()
        {
            RuleFor(x => x.RoleName)
                .MaximumLength(64).WithMessage("Role name must be less than 64 characters")
                .Matches(@"^[\p{L}\s0-9]+$").WithMessage("Invalid Role Name")
                .NotEmpty().WithMessage("RoleName is required");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        }
    }
}
