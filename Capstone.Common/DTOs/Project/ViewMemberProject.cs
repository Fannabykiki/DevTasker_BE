namespace Capstone.Common.DTOs.Project
{
	public class ViewMemberProject
	{
		public Guid MemberId { get; set; }
		public Guid UserId { get; set; }
		public Guid? RoleId { get; set; }
		public Guid ProjectId { get; set; }
		public bool IsOwner { get; set; }
	}
	public class ListViewMemberProject
	{
		public List<ViewMemberProject> Members { get; set; }
		public int numberOfMember { get; set; }
	}


}
