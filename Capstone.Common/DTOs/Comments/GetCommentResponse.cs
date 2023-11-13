using Capstone.Common.DTOs.User;

namespace Capstone.Common.DTOs.Comments
{
    public class GetCommentResponse
    {
        public Guid CommentId { get; set; }
        public string? Content { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public Guid TaskId { get; set; }
        public GetUserCommentResponse User { get; set; } // 1 comment just create by 1 user
    }
}
