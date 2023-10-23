using Capstone.Common.DTOs.Board;
using Capstone.Common.DTOs.Iteration;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.IterationValidator
{
    public class UpdateBoardValidator : AbstractValidator<UpdateBoardRequest>
    {
        public UpdateBoardValidator() 
        {
            RuleFor(request => request.Title).NotEmpty().WithMessage("Board title cannot be empty");
           
        }
    }
}
