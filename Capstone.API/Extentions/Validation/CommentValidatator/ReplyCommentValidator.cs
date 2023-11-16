using Capstone.Common.DTOs.Comments;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.CommentValidatator
{
    public class ReplyCommentValidator : AbstractValidator<ReplyCommentRequest>
    {
        public ReplyCommentValidator() {
            RuleFor(x => x.Content)
              .NotEmpty().WithMessage("Content is required");
        }
    }
}
