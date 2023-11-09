using Capstone.Common.DTOs.Schema;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.PermissionSchemaValidator
{
    public class UpdateSchemaValidator : AbstractValidator<UpdateSchemaRequest>
    {
        public UpdateSchemaValidator() {
            RuleFor(x => x.SchemaName)
            .MaximumLength(64).WithMessage("User name must be less than 64 characters")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Invalid Role Name")
            .NotEmpty().WithMessage("Schema name is required.")
            .MaximumLength(100).WithMessage("Schema name must not exceed 100 characters.");

            RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
