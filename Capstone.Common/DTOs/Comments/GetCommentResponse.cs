using Capstone.Common.DTOs.User;

namespace Capstone.Common.DTOs.Comments
{
    public class GetCommentResponse
    {
        public Guid CommentId { get; set; }
        public string? Content { get; set; }
        public string CreateAt { get; set; }
        public string? DeleteAt { get; set; }
        public string? UpdateAt { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? ReplyTo { get; set; }

        public GetUserCommentResponse User { get; set; } // 1 comment just create by 1 user
        public List<GetCommentResponse> SubComments { get; set; }
    }
}
