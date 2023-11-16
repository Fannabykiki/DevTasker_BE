using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.Iteration;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.CommentValidatator
{
    public class CreateCommentValidataor : AbstractValidator<CreateCommentRequest>
    {
        public CreateCommentValidataor()
        {
            RuleFor(x => x.Content)
              .NotEmpty().WithMessage("Content is required");

            RuleFor(x => x.TaskId)
              .NotEqual(Guid.Empty).WithMessage("TaskId is required");
        }
    }
}
