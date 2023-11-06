using Capstone.Common.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.RoleService
{
    public interface IRoleService
    {
        Task<List<GetAllRoleResponse>> GetAllSystemRole();
    }
}
