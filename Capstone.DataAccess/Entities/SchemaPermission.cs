namespace Capstone.DataAccess.Entities
{
	public class SchemaPermission
	{
        public Guid SchemaId { get; set; }
        public Guid PermissionId { get; set; }
        public Guid? RoleId { get; set; }
        public Schema Schema { get; set; }
        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
