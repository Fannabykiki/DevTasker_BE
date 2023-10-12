namespace Capstone.Common.DTOs.Project
{
	public class CreateRoleRequest
	{
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string SchemaName { get; set; }
        public string SchemaDes { get; set; }
        public List<Guid> PermissionId { get; set; }
    }
}
