
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
        private readonly IPermissionSchemaRepository _permissionSchemaRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper, IProjectRepository projectRepository, IPermissionSchemaRepository permissionSchemaRepository)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _projectRepository = projectRepository;
            _permissionSchemaRepository = permissionSchemaRepository;
        }
        public async Task<List<GetRoleRecord>> GetAllSystemRole(bool mode)
        {
            using var transaction = _projectRepository.DatabaseTransaction();
            try
            {
                var result = new List<GetRoleRecord>();
                IEnumerable<Role>? roles;
                if(mode == true)
                 roles = await _roleRepository.GetAllWithOdata(x => x.IsDelete != true, x => x.SchemaPermissions);
                else
                 roles = await _roleRepository.GetAllWithOdata(x => x.IsDelete == true, x => x.SchemaPermissions);
                var roleRecords = new List<GetRoleRecord>();
                foreach (var role in roles)
                {
                    var newRoleRecord = new GetRoleRecord();
                    newRoleRecord.RoleId = role.RoleId;
                    newRoleRecord.RoleName = role.RoleName;
                    newRoleRecord.Description = role.Description;

                    HashSet<GetProjectUsedResponse> projectUseds = new HashSet<GetProjectUsedResponse>();

                    var listSchemaPermissions = role.SchemaPermissions.Select(x => x.SchemaId);
                    HashSet<Guid> roleSchemasId = new HashSet<Guid>();
                    foreach (var roleSchema in listSchemaPermissions)
                    {
                        roleSchemasId.Add(roleSchema);
                    }
                    foreach (var schemaId in roleSchemasId)
                    {
                        var projects = await _projectRepository.GetAllWithOdata(x => x.SchemasId == schemaId, x => x.Status);
                        var projectUsed = _mapper.Map<List<GetProjectUsedResponse>>(projects);
                        foreach (var project in projectUsed)
                        {
                            projectUseds.Add(project);
                        }
                    }
                    newRoleRecord.ProjectsUsed = projectUseds.ToList();
                    roleRecords.Add(newRoleRecord);
                }

                if (roles == null) throw new Exception();

                result = roleRecords;
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
                    Description = createRoleRequest.Description,
                    IsDelete = false
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

        public async Task<List<GetRoleResponse>> GetRolesByProjectId(Guid projectId)
        {
            var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null);
            if (project == null) return null;
            var schemas = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == project.SchemasId,x => x.Role);
            var roles = new List<GetRoleResponse>();
            HashSet<Role> Role = new HashSet<Role>();
            foreach (var schema in schemas)
            {
                if (schema.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B") ||schema.RoleId == Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430")) continue;
                Role.Add(schema.Role);
            }
            foreach (var role in Role)
            {
                roles.Add(new GetRoleResponse
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description,
                });
            }
            return roles;
        }

        public async Task<List<GetRoleResponse>> GetRoleToEdit(Guid schemaId, EditSchemaRoleRequest editSchemaRoleRequest, bool mode)
        {
            HashSet<Role> Roles = new HashSet<Role>();
            foreach (var permission in editSchemaRoleRequest.PermissionIds)
            {
                var permissonSchema = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == schemaId && x.PermissionId == permission,x => x.Role);
                foreach(var role in permissonSchema)
                {
                    if (role.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B") || role.RoleId == Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430")) continue;
                    Roles.Add(role.Role);
                }
            }
            if (mode == false)
            {
                var roles = new List<GetRoleResponse>();
                foreach (var role in Roles)
                {
                    if (role.IsDelete != true)
                        roles.Add(_mapper.Map<GetRoleResponse>(role));
                    else continue;
                }
                return roles;
            }
            else
            {
                var rolegrant = await _roleRepository.GetAllWithOdata(x => x.IsDelete != true, null);
                rolegrant = rolegrant.Except(Roles);
                return _mapper.Map<List<GetRoleResponse>>(rolegrant.Where(x => x.RoleId != Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B") && x.RoleId != Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430")));
            }
            
        }
    }
}
