using AutoMapper;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Schema;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.PermissionSchemaService
{
    public class PermissionSchemaService : IPermissionSchemaService
    {
        private readonly IPermissionSchemaRepository _permissionSchemaRepository;
        private readonly ISchemaRepository _schemaRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public PermissionSchemaService(ILoggerManager logger, IPermissionSchemaRepository permissionSchemaRepository, ISchemaRepository schemaRepository, IRoleRepository roleRepository, IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionSchemaRepository = permissionSchemaRepository;
            _schemaRepository = schemaRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IEnumerable<GetAllPermissionSchemaResponse>> GetAllSchema()
        {
            var schemas = await _schemaRepository.GetAllWithOdata(x => true, null);
            return _mapper.Map<List<GetAllPermissionSchemaResponse>>(schemas);
        }

        public async Task<GetPermissionSchemaByIdResponse> GetPermissionSchemaById(Guid schemaId)
        {
            var schemas = await _schemaRepository.GetAsync(x => x.SchemaId == schemaId, null);
            var permissionSchemas = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == schemaId, x => x.Permission);
            var permissions = await _permissionRepository.GetAllWithOdata(x => true, null);
            var permissionRoles = new List<PermissionRolesDTO>();

            foreach (var permission in permissions)
            {
                var per = permissionSchemas.Where(x => x.PermissionId == permission.PermissionId);
                var roles = per.Select(x => x.RoleId);
                var permissionInRoles = new List<RoleInPermissionDTO>();
                foreach (var item in roles)
                {
                    var role = await _roleRepository.GetAsync(x => x.RoleId == item, null);
                    permissionInRoles.Add(new RoleInPermissionDTO
                    {
                        Description = role.Description,
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
                    });
                }
                permissionRoles.Add(new PermissionRolesDTO
                {
                    PermissionId = permission.PermissionId,
                    Name = permission.Name,
                    Description = permission.Description,
                    Roles = permissionInRoles
                });
            }

            var result = new GetPermissionSchemaByIdResponse
            {
                SchemaId = schemaId,
                SchemaName = schemas.SchemaName,
                Description = schemas.Description,
                rolePermissions = permissionRoles
            };

            return result;
        }

        public async Task<bool> CreateNewPermissionSchema(CreateNewSchemaRequest request)
        {
            using var transaction = _permissionSchemaRepository.DatabaseTransaction();
            try
            {
                var schema = new Schema
                {
                    SchemaName = request.SchemaName,
                    Description = request.Description
                };
                var newSchema = await _schemaRepository.CreateAsync(schema);
                var permissions = _permissionRepository.GetAllAsync(x => x.PermissionId != null, null);
                foreach (var permission in permissions)
                {
                    await _permissionSchemaRepository.CreateAsync(new SchemaPermission
                    {
                        SchemaId = newSchema.SchemaId,
                        PermissionId = permission.PermissionId,
                        RoleId = Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B") //PO
                    });
                    await _permissionSchemaRepository.CreateAsync(new SchemaPermission
                    {
                        SchemaId = newSchema.SchemaId,
                        PermissionId = permission.PermissionId,
                        RoleId = Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430") //Admin of system
                    });
                }
                _permissionSchemaRepository.SaveChanges();
                _schemaRepository.SaveChanges();


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
        public async Task<bool> UpdateSchema(Guid schemaId, UpdateSchemaRequest request)
        {
            using var transaction = _permissionSchemaRepository.DatabaseTransaction();
            try
            {
                var schema = await _schemaRepository.GetAsync(x => x.SchemaId == schemaId, null);
                if (schema == null) { return false; }

                schema.SchemaName = request.SchemaName ?? schema.SchemaName;
                schema.Description = request.Description ?? schema.Description;

                _schemaRepository.UpdateAsync(schema);
                _schemaRepository.SaveChanges();

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
        public async Task<bool> UpdateSchemaPermissionRoles(Guid schemaId, UpdatePermissionSchemaRequest request)
        {
            using var transaction = _permissionSchemaRepository.DatabaseTransaction();
            try
            {
                var schema = await _schemaRepository.GetAsync(x => x.SchemaId == schemaId, null);
                if (schema == null) return false;

                foreach (var permission in request.rolePermissions)
                {
                    var per = _permissionRepository.GetAllAsync(x => x.PermissionId == permission.PermissionId, null);
                    if (per == null) return false;

                    var listCurentRole = _permissionSchemaRepository.GetAllAsync(x => x.PermissionId == permission.PermissionId && x.SchemaId == schemaId, null).Select(x => x.RoleId);

                    foreach (var curentRole in listCurentRole)
                    {
                        if (!permission.RoleIds.Contains((Guid)curentRole))
                        {
                            var deletedRole = _permissionSchemaRepository.GetAllAsync(x => x.SchemaId == schemaId && x.PermissionId == permission.PermissionId && x.RoleId == curentRole, null);
                            await _permissionSchemaRepository.DeleteAsync((SchemaPermission)deletedRole);
                        }
                    }

                    foreach (var role in permission.RoleIds)
                    {
                        var rol = _roleRepository.GetAllAsync(x => x.RoleId == role, null);
                        if (rol == null) return false;

                        var permissionSchema = await _permissionSchemaRepository.GetAsync(x => x.SchemaId == schemaId && x.PermissionId == permission.PermissionId && x.RoleId == role, null);
                        if (permissionSchema == null)
                        {
                            await _permissionSchemaRepository.CreateAsync(new SchemaPermission
                            {
                                SchemaId = schemaId,
                                PermissionId = permission.PermissionId,
                                RoleId = role
                            });
                        }
                    }
                }
                _permissionSchemaRepository.SaveChanges();

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
