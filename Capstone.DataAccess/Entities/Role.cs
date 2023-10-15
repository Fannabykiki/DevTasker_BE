using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Role
    {
        [Key]
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public Guid ProjectId { get; set; }
        public List<PermissionSchema> PermissionSchemas { get; set; }
        public List<ProjectMember>? ProjectMember { get; set; }
        public Project Project { get; set; }
    }
}
