using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.Project
{
	public class CreateProjectRespone : BaseResponse
	{
		public Guid ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string Description { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public Guid CreateBy { get; set; }
		public string CreateAt { get; set; }
		public Guid SchemasId { get; set; }
		public Guid StatusId { get; set; }
		public DateTime? DeleteAt { get; set; }
		public DateTime? ExpireAt { get; set; }
		public bool PrivacyStatus { get; set; } // false: Private , true: Public
	}
}
