using Capstone.Common.DTOs.Board;
using Capstone.Common.DTOs.Iteration;
using FluentValidation;

public class CreateBoardValidatator : AbstractValidator<CreateBoardRequest>
{
    public CreateBoardValidatator()
    {
        RuleFor(request => request.Title).NotEmpty().WithMessage("Board title cannot be empty");
       
    }
}