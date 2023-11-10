using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Schema
    {
        public Guid SchemaId { get; set; }
        public string SchemaName { get; set; }
        public string? Description { get; set; }
        public List<SchemaPermission> SchemaPermissions { get; set; }
    }
}
