namespace Capstone.Common.DTOs.User
{
	public class UpdateMemberRoleRequest
	{
		public Guid MemberId { get; set; }

		public Guid RoleId { get; set; }
    }
}
