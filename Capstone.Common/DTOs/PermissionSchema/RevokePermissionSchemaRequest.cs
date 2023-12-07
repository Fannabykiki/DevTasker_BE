using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.PermissionSchema
{
    public class RevokePermissionSchemaRequest
    {
		public Guid SchemaId { get; set; }
		public Guid PermissionId { get; set; }
        public List<Guid> RoleIds { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
