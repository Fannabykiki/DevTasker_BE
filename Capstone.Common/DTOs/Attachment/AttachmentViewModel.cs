namespace Capstone.Common.DTOs.Attachment
{
	public class AttachmentViewModel
	{
		public Guid AttachmentId { get; set; }
		public string Title { get; set; }
		public string CreateAt { get; set; }
		public string? DeleteAt { get; set; }
		public string? ExprireTime { get; set; }
		public Guid? TaskId { get; set; }
		public string TaskTitle { get; set; }
		public string CreateBy { get; set; }
		public bool? IsDeleted { get; set; }
	}
}
