using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.Authencation
{
    public class ExternalLoginValidator : AbstractValidator<ExternalLoginRequest>
    {
        public ExternalLoginValidator() {
            RuleFor(x => x.code).NotEmpty().WithMessage("Credential is required");
        }
    }
}
