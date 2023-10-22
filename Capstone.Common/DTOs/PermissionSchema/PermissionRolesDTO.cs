using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.PermissionSchema
{
    public class PermissionRolesDTO
    {
        public Guid PermissionId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<RoleInPermissionDTO> Roles { get; set; }
    }
}
