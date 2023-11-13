using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Schema
    {
        public Guid SchemaId { get; set; }
        public string SchemaName { get; set; }
        public string? Description { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? ExprireTime { get; set; }
        public List<SchemaPermission> SchemaPermissions { get; set; }
    }
}
