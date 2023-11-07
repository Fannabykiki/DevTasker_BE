
using AutoMapper;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;


        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetRoleResponse>> GetAllSystemRole()
        {
            var roles = await _roleRepository.GetAllWithOdata(x => true, null);
            if (roles == null) return null;
            return _mapper.Map<List<GetRoleResponse>>(roles);
        }

        public async Task<GetRoleResponse> UpdateSystemRole(Guid roleId, UpdateRoleRequest request)
        {
            using var transaction = _roleRepository.DatabaseTransaction();
            try
            {
                var role = await _roleRepository.GetAsync(x => x.RoleId == roleId, null);
                role.RoleName = request.RoleName;
                role.Description = request.Description ?? role.Description;

                _roleRepository.UpdateAsync(role);
                _roleRepository.SaveChanges();

                return _mapper.Map<GetRoleResponse>(role);
            }
            catch (Exception)
            {
                transaction.RollBack();
                return null;
            }
        }
        public async Task<GetRoleResponse> CreateProjectRole(CreateRoleRequest createRoleRequest)
        {
            using var transaction = _roleRepository.DatabaseTransaction();
            try
            {
                var newRoleRequest = new Role
                {
                    RoleId = Guid.NewGuid(),
                    RoleName = createRoleRequest.RoleName,
                    Description = createRoleRequest.Description
                };

                await _roleRepository.CreateAsync(newRoleRequest);
                await _roleRepository.SaveChanges();

                transaction.Commit();
                return _mapper.Map<GetRoleResponse>(newRoleRequest);
            }
            catch (Exception)
            {
                transaction.RollBack();
                return null;
            }
        }
    }
}
