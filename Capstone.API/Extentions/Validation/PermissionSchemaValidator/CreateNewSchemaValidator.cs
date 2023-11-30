using Capstone.Common.DTOs.PermissionSchema;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.PermissionSchemaValidator
{
    public class CreateNewSchemaValidator : AbstractValidator<CreateNewSchemaRequest>
    {
        public CreateNewSchemaValidator()
        {
            RuleFor(x => x.SchemaName)
            .Matches(@"^[\p{L}\s0-9]+$").WithMessage("Invalid Schema Name")
            .NotEmpty().WithMessage("Schema name is required.")
            .MaximumLength(100).WithMessage("Schema name must not exceed 100 characters.");

            RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }

}
