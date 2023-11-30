using Capstone.Common.DTOs.Project;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.ProjectValidator
{
    public class UpdatePermissionSchemaValidator : AbstractValidator<UpdatePermissionSchemaRequest>
    {
        public UpdatePermissionSchemaValidator() {
            RuleFor(x => x.SchemaId).NotNull().WithMessage("Schema ID is require");
        }
    }
}
