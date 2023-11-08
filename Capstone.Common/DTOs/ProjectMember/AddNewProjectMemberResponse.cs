using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.ProjectMember
{
	public class AddNewProjectMemberResponse : BaseResponse
	{
		public Guid MemberId { get; set; }
		public Guid UserId { get; set; }
		public Guid? RoleId { get; set; }
		public Guid ProjectId { get; set; }
		public bool IsOwner { get; set; }
		public string StatusName { get; set; }
	}
}
