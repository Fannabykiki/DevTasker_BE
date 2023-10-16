using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class PermissionSchema
    {
        public string SchemaName { get; set; }
        public string Description { get; set; }
        public Guid? PermissionId { get; set; }
        public Guid? RoleId { get; set; }
        public Permission? Permission { get; set; } //1 Schema has many permission 
        public Role Role { get; set; } //1 Schema has many role
    }
}
