using Capstone.Common.DTOs.Role;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.RoleValidator
{
    public class UpdateRoleValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.RoleName)
                .MaximumLength(64).WithMessage("User name must be less than 64 characters")
                .Matches(@"^[\p{L}\s0-9]+$").WithMessage("Invalid Role Name");

            RuleFor(x => x.Description)
              .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
              .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
