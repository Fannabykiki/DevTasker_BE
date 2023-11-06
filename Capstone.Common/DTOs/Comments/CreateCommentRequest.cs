namespace Capstone.Common.DTOs.Comments
{
    public class CreateCommentRequest
    {
        public string? Content { get; set; }
        public Guid TaskId { get; set; }
        public Guid ByUser { get; set; }
    }
}
