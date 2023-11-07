using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Role
{
    public class UpdateRoleRequest
    {
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
