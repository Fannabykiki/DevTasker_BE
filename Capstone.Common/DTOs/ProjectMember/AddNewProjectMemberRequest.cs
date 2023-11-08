namespace Capstone.Common.DTOs.ProjectMember
{
	public class AddNewProjectMemberRequest
	{
		public Guid UserId { get; set; }
		public Guid ProjectId { get; set; }
		public Guid? StatusId { get; set; }
	}
}
