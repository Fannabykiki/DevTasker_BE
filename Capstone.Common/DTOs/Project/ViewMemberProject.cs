namespace Capstone.Common.DTOs.Project
{
	public class ViewMemberProject
	{
		public Guid MemberId { get; set; }
		public Guid UserId { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public Guid? RoleId { get; set; }
		public Guid ProjectId { get; set; }
		public string? RoleName { get; set; }
		public bool IsOwner { get; set; }
	}
}
