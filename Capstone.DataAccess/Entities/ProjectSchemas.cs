namespace Capstone.DataAccess.Entities
{
	public class ProjectSchemas
	{
        public Guid ProjectId { get; set; }
        public Guid SchemaId { get; set; }
        public Project Project { get; set; }
        public PermissionSchema PermissionSchema { get; set; }
    }
}
