using Capstone.Common.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.PermissionSchema
{
    public class RolePermissionDTO
    {
        public Guid PermissionId { get; set; }
        public List<Guid> RoleIds { get; set; }
    }
}
