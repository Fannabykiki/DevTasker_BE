using Capstone.Common.DTOs.Paging;
using Capstone.Common.DTOs.User;

namespace Capstone.Common.DTOs.Project
{
	public class GetAllProjectResponse
    {
		public Guid ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string Description { get; set; }
		public string ProjectStatus { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public UserResponse? Manager { get; set; }
		public List<UserResponse>? Member { get; set; }
		public DateTime CreateAt { get; set; }
		public DateTime? DeleteAt { get; set; }
		public DateTime? ExpireAt { get; set; }
		public bool PrivacyStatus { get; set; } // false: Private , true: Pu
    }
}
