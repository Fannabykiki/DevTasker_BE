using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
	public class Role
	{
		[Key]
		public Guid RoleId { get; set; }
		public string RoleName { get; set; }
		public string Description { get; set; }
		public bool IsDelete { get; set; }
		public List<SchemaPermission>? SchemaPermissions { get; set; }
		public List<ProjectMember>? ProjectMember { get; set; }
	}
}
