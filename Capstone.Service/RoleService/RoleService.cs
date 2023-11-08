
using AutoMapper;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Google.Apis.Drive.v3.Data;
using Org.BouncyCastle.Asn1.Ocsp;
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
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;


        public RoleService(IRoleRepository roleRepository, IMapper mapper, IProjectRepository projectRepository)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<GetAllRoleReponse> GetAllSystemRole(bool mode)
        {
            using var transaction = _projectRepository.DatabaseTransaction();
            try
            {
                var result = new GetAllRoleReponse();
                IEnumerable<Role>? roles;
                if(mode == true)
                 roles = await _roleRepository.GetAllWithOdata(x => x.IsDelete != true, x => x.SchemaPermissions);
                else
                 roles = await _roleRepository.GetAllWithOdata(x => x.IsDelete == true, x => x.SchemaPermissions);
                var roleRecords = new List<GetRoleRecord>();
                foreach (var role in roles)
                {
                    var newRoleRecord = new GetRoleRecord();
                    newRoleRecord.Role = _mapper.Map<GetRoleResponse>(role);
                    HashSet<GetProjectUseRoleResponse> projectUseds = new HashSet<GetProjectUseRoleResponse>();

                    var listSchemaPermissions = role.SchemaPermissions.Select(x => x.SchemaId);
                    HashSet<Guid> roleSchemasId = new HashSet<Guid>();
                    foreach (var roleSchema in listSchemaPermissions)
                    {
                        roleSchemasId.Add(roleSchema);
                    }
                    foreach (var schemaId in roleSchemasId)
                    {
                        var projects = await _projectRepository.GetAllWithOdata(x => x.SchemasId == schemaId, x => x.Status);
                        var projectUsed = _mapper.Map<List<GetProjectUseRoleResponse>>(projects);
                        foreach (var project in projectUsed)
                        {
                            projectUseds.Add(project);
                        }
                    }
                    newRoleRecord.ProjectsUsed = projectUseds.ToList();
                    roleRecords.Add(newRoleRecord);
                }

                if (roles == null) throw new Exception();
                result.pagination = new Common.DTOs.Paging.Pagination
                {
                    TotalRecords = roleRecords.Count()
                };
                result.roleRecords = roleRecords;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                transaction.RollBack();
                return null;
            }
            
        }

        public async Task<GetRoleResponse> UpdateSystemRole(Guid roleId, UpdateRoleRequest request)
        {
            using var transaction = _roleRepository.DatabaseTransaction();
            try
            {
                var role = await _roleRepository.GetAsync(x => x.RoleId == roleId, null);
                role.RoleName = request.RoleName;
                role.Description = request.Description ?? role.Description;

                await _roleRepository.UpdateAsync(role);
                await _roleRepository.SaveChanges();
                transaction.Commit();
                return _mapper.Map<GetRoleResponse>(role);
            }
            catch (Exception)
            {
                transaction.RollBack();
                return null;
            }
        }
        public async Task<GetRoleResponse> CreateProjectRole(CreateNewRoleRequest createRoleRequest)
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

        public async Task<GetRoleResponse> GetSystemRoleByName(string roleName)
        {
            var role = await _roleRepository.GetAsync(x => x.RoleName.Trim().ToLower().Equals(roleName.Trim().ToLower()),null);
            return _mapper.Map<GetRoleResponse>(role);
        }

        public async Task<GetRoleResponse> GetSystemRoleById(Guid roleId)
        {
            var role = await _roleRepository.GetAsync(x => x.RoleId == roleId, null);
            return _mapper.Map<GetRoleResponse>(role);
        }

        public async Task<bool> RemoveSystemRoleAsync(Guid roleId)
        {
            using var transaction = _projectRepository.DatabaseTransaction();
            try
            {
                var role = await _roleRepository.GetAsync(x => x.RoleId == roleId, null);
                role.RoleName = role.RoleName.Trim() + " (Deleted)";
                role.IsDelete = true;

                await _roleRepository.UpdateAsync(role);
                await _roleRepository.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                transaction.RollBack();
                return false;
            }
        }
    }
}
