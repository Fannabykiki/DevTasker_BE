namespace Capstone.Common.DTOs.Comments
{
    public class ReplyCommentRequest
    {
        public Guid CommentId { get; set; }
        public string? Content { get; set; }
    }
}
