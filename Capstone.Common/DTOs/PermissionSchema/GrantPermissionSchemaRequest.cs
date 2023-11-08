using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.PermissionSchema
{
    public class GrantPermissionSchemaRequest
    {
        public Guid SchemaId { get; set; }
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; }
    }
}
