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

        Task<GetAllRoleReponse> GetAllSystemRole(int limit, int page,bool mode);
        Task<GetRoleResponse> GetSystemRoleByName(string roleName);
        Task<GetRoleResponse> GetSystemRoleById(Guid roleId);
        Task<GetRoleResponse> CreateProjectRole(CreateNewRoleRequest createRoleRequest);
        Task<GetRoleResponse> UpdateSystemRole(Guid roleId, UpdateRoleRequest request);
        Task<bool> RemoveSystemRoleAsync(Guid roleId);
    }
}
