using AutoMapper;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Schema;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Google.Apis.Drive.v3.Data;
using System.Data;

namespace Capstone.Service.PermissionSchemaService
{
	public class PermissionSchemaService : IPermissionSchemaService
    {
        private readonly IPermissionSchemaRepository _permissionSchemaRepository;
        private readonly ISchemaRepository _schemaRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;


        public PermissionSchemaService(ILoggerManager logger, IPermissionSchemaRepository permissionSchemaRepository, ISchemaRepository schemaRepository, IRoleRepository roleRepository, IPermissionRepository permissionRepository, IMapper mapper,IProjectRepository projectRepository)
        {
            _permissionSchemaRepository = permissionSchemaRepository;
            _schemaRepository = schemaRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _logger = logger;
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<List<GetSchemaResponse>> GetAllSchema(bool mode)
        {
            IEnumerable<Schema>? schemas;
            if (mode) {
                schemas = await _schemaRepository.GetAllWithOdata(x => x.IsDelete != true, null);
            }
            else
            {
                schemas = await _schemaRepository.GetAllWithOdata(x => x.IsDelete == true, null);
            }
            var results = _mapper.Map<List<GetSchemaResponse>>(schemas);
            foreach(var schema in results)
            {
                var projects = await _projectRepository.GetAllWithOdata(x => x.SchemasId == schema.SchemaId, x => x.Status);
                if (projects != null)
                {
                    var projectUsed = projects.Select(p => new GetProjectUsedResponse
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        Description = p.Description,
                        ProjectStatus = p.Status.Title
                    }).ToList();
                    schema.ProjectsUsed = (List<GetProjectUsedResponse>?)projectUsed;
                }
                
            }
            return results;
        }

        public async Task<GetPermissionSchemaByIdResponse> GetPermissionSchemaById(Guid schemaId, Guid? projectId)
        {
            if (projectId != null)
            {
                var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null);
                schemaId = project.SchemasId;
            }
            var schemas = await _schemaRepository.GetAsync(x => x.SchemaId == schemaId && x.IsDelete != true, null);
            if (schemas == null) return null;
            
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
                    Description = request.Description,
                    IsDelete = false
                };
                var newSchema = await _schemaRepository.CreateAsync(schema);
                var permissions = _permissionRepository.GetAllAsync(x => true, null);
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
                await _permissionSchemaRepository.SaveChanges();
                await _schemaRepository.SaveChanges();


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
                var schema = await _schemaRepository.GetAsync(x => x.SchemaId == schemaId && x.IsDelete != true, null);
                if (schema == null) { return false; }

                schema.SchemaName = request.SchemaName ?? schema.SchemaName;
                schema.Description = request.Description ?? schema.Description;

                await _schemaRepository.UpdateAsync(schema);
                await _schemaRepository.SaveChanges();

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
        public async Task<bool> GrantSchemaPermissionRoles(Guid schemaId, GrantPermissionSchemaRequest request, Guid userId)
        {
            using var transaction = _permissionSchemaRepository.DatabaseTransaction();
            try
            {
                if (request.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B") ||
                        request.RoleId == Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430")) return false;
                var schemaPermission = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == schemaId, null);

                if (request.ProjectId != null)
                {

                    var project = await _projectRepository.GetAsync(x => x.ProjectId == request.ProjectId, x => x.Schemas);

                    if ((schemaId == Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990") && project.SchemasId == Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990")) && userId != Guid.Parse("AFA06CDD-7713-4B81-9163-C45556E4FA4C"))
                    {
                        var Schema = new Schema
                        {
                            SchemaName = "Schema " + project.ProjectName,
                            Description = "Permission Schema for project\" " + project.ProjectName + "\"",
                            IsDelete = false
                        };
                        var newSchema = await _schemaRepository.CreateAsync(Schema);
                        
                        foreach (var item in schemaPermission)
                        {
                            item.SchemaId = newSchema.SchemaId;
                            await _permissionSchemaRepository.CreateAsync(item);
                        }
                        await _permissionSchemaRepository.SaveChanges();
                        await _schemaRepository.SaveChanges();
                        project.SchemasId = newSchema.SchemaId;
                        await _projectRepository.UpdateAsync(project);
                        await _projectRepository.SaveChanges();
                        if (project.SchemasId != Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"))
                        {
                            var currentSchema = await _schemaRepository.GetAsync(x => x.SchemaId == project.SchemasId, x => x.SchemaPermissions);

                            foreach (var item in currentSchema.SchemaPermissions)
                            {
                                await _permissionSchemaRepository.DeleteAsync(item);
                            }
                            await _permissionSchemaRepository.SaveChanges();
                            await _schemaRepository.DeleteAsync(currentSchema);
                            await _schemaRepository.SaveChanges();
                        }


                        schemaId = newSchema.SchemaId;
                    }
                    else if (project.SchemasId != schemaId && project.SchemasId != Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"))
                    {
                        var currentSchema = await _schemaRepository.GetAsync(x => x.SchemaId == project.SchemasId, x => x.SchemaPermissions);

                        currentSchema.SchemaName = "Schema " + project.ProjectName;
                        currentSchema.Description = "Permission Schema cloned from \"" + schemaPermission.First().Schema.SchemaName + "\"";
                        currentSchema.IsDelete = false;
                        await _schemaRepository.UpdateAsync(currentSchema);
                        await _schemaRepository.SaveChanges();


                        var currentSchemaPermission = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == project.SchemasId, null);
                        foreach (var item in currentSchemaPermission)
                        {
                            await _permissionSchemaRepository.DeleteAsync(item);
                        }
                        await _permissionSchemaRepository.SaveChanges();
                        foreach (var item in schemaPermission)
                        {
                            item.SchemaId = project.SchemasId;
                            await _permissionSchemaRepository.CreateAsync(item);
                        }
                        await _permissionSchemaRepository.SaveChanges();
                        schemaId = project.SchemasId;
                    }
                    else
                    {
                        var currentSchema = await _schemaRepository.GetAsync(x => x.SchemaId == project.SchemasId, x => x.SchemaPermissions);

                        currentSchema.Description = "Permission Schema for project\" " + project.ProjectName + "\"";

                        await _schemaRepository.UpdateAsync(currentSchema);
                        await _schemaRepository.SaveChanges();
                    }
                }



                foreach (var permission in request.PermissionIds)
                {
                    var permissionRole = schemaPermission.Where(x => x.PermissionId == permission && x.RoleId == request.RoleId);
                    if(permissionRole.Any() == false)
                    {
                        var newPermissionRole = new SchemaPermission
                        {
                            SchemaId = schemaId,
                            PermissionId = permission,
                            RoleId = request.RoleId
                        };
                        await _permissionSchemaRepository.CreateAsync(newPermissionRole);
                    }
                }
                await _permissionSchemaRepository.SaveChanges();
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

        public async Task<GetSchemaResponse> GetSchemaById(Guid SchemaId)
        {
            var schema = await _schemaRepository.GetAsync(x => x.SchemaId == SchemaId && x.IsDelete != true, null);
            return _mapper.Map<GetSchemaResponse>(schema);
        }

        public async Task<bool> RevokeSchemaPermissionRoles(Guid schemaId, RevokePermissionSchemaRequest request, Guid userId)
        {
            using var transaction = _permissionSchemaRepository.DatabaseTransaction();
            try
            {
                if(request.ProjectId != null)
                {
                    var project = await _projectRepository.GetAsync(x => x.ProjectId == request.ProjectId, null);
                    var SchemaPermission = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == schemaId, null);
                    if ((schemaId == Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990") || project.SchemasId == Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990")) && userId != Guid.Parse("AFA06CDD-7713-4B81-9163-C45556E4FA4C"))
                    {


                        var Schema = new Schema
                        {
                            SchemaName = "Schema " + project.ProjectName,
                            Description = "Permission Schema for project\" " + project.ProjectName + "\"",
                            IsDelete = false
                        };
                        var newSchema = await _schemaRepository.CreateAsync(Schema);
                        foreach (var item in SchemaPermission)
                        {
                            item.SchemaId = newSchema.SchemaId;
                            await _permissionSchemaRepository.CreateAsync(item);
                        }
                        await _permissionSchemaRepository.SaveChanges();
                        await _schemaRepository.SaveChanges();
                        project.SchemasId = newSchema.SchemaId;
                        await _projectRepository.UpdateAsync(project);
                        await _projectRepository.SaveChanges();
                        if (project.SchemasId != Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"))
                        {
                            var currentSchema = await _schemaRepository.GetAsync(x => x.SchemaId == project.SchemasId, x => x.SchemaPermissions);

                            foreach (var item in currentSchema.SchemaPermissions)
                            {
                                await _permissionSchemaRepository.DeleteAsync(item);
                            }
                            await _permissionSchemaRepository.SaveChanges();
                            await _schemaRepository.DeleteAsync(currentSchema);
                            await _schemaRepository.SaveChanges();
                        }
                        schemaId = newSchema.SchemaId;
                    }
                    else if (project.SchemasId != schemaId && project.SchemasId != Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"))
                    {
                        var currentSchema = await _schemaRepository.GetAsync(x => x.SchemaId == project.SchemasId, x => x.SchemaPermissions);

                        currentSchema.SchemaName = "Schema " + project.ProjectName;
                        currentSchema.Description = "Permission Schema cloned from \"" + SchemaPermission.First().Schema.SchemaName + "\"";
                        currentSchema.IsDelete = false;
                        await _schemaRepository.UpdateAsync(currentSchema);
                        await _schemaRepository.SaveChanges();


                        var currentSchemaPermission = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == project.SchemasId, null);
                        foreach (var item in currentSchemaPermission)
                        {
                            await _permissionSchemaRepository.DeleteAsync(item);
                        }
                        await _permissionSchemaRepository.SaveChanges();
                        foreach (var item in SchemaPermission)
                        {
                            item.SchemaId = project.SchemasId;
                            await _permissionSchemaRepository.CreateAsync(item);
                        }
                        await _permissionSchemaRepository.SaveChanges();
                        schemaId = project.SchemasId;
                    }
                    else
                    {
                        var currentSchema = await _schemaRepository.GetAsync(x => x.SchemaId == project.SchemasId, x => x.SchemaPermissions);

                        currentSchema.Description = "Permission Schema for project\" " + project.ProjectName + "\"";

                        await _schemaRepository.UpdateAsync(currentSchema);
                        await _schemaRepository.SaveChanges();
                    }
                }
                


                var schemaPermission = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == schemaId, null);
                foreach (var role in request.RoleIds)
                {
                    if(role == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B") || 
                        role == Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430")) continue;

                    var permissionRole = schemaPermission.First(x => x.RoleId == role && x.PermissionId == request.PermissionId);
                    if (permissionRole != null)
                    {
                        await _permissionSchemaRepository.DeleteAsync(permissionRole);
                    }
                }
                await _permissionSchemaRepository.SaveChanges();
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

        public async Task<GetSchemaResponse> GetSchemaByName(string schemaName)
        {
            var schema = await _schemaRepository.GetAsync(x => x.SchemaName.Trim().ToLower().Equals(schemaName.Trim().ToLower()) && x.IsDelete != true, null);
            return _mapper.Map<GetSchemaResponse>(schema);
        }

        public async Task<bool> RemoveSchemaAsync(Guid SchemaId)
        {
            using var transaction = _projectRepository.DatabaseTransaction();
            try
            {
                if(SchemaId == Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"))
                {
                    return false;
                }
                var schema = await _schemaRepository.GetAsync(x => x.SchemaId == SchemaId, null);
                schema.SchemaName = schema.SchemaName.Trim() + " (Deleted)";
                schema.IsDelete = true;
                schema.DeleteAt= DateTime.Now;

                await _schemaRepository.UpdateAsync(schema);
                await _schemaRepository.SaveChanges();
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

        public async Task<List<GetProjectSchemasResponse>> GetProjectSchemas(Guid projectId)
        {
            var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null);
            var schemas = await _schemaRepository.GetAllWithOdata(x => x.IsDelete != true, null);
            
            var results = _mapper.Map<List<GetProjectSchemasResponse>>(schemas);
            foreach (var schema in results)
            {
                schema.isCurrentProjectSchema = project.SchemasId == schema.SchemaId ? true : false;
                var projects = await _projectRepository.GetAllWithOdata(x => x.SchemasId == schema.SchemaId, x => x.Status);
                if (projects != null)
                {
                    var projectUsed = projects.Select(p => new GetProjectUsedResponse
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        Description = p.Description,
                        ProjectStatus = p.Status.Title
                    }).ToList();
                    schema.ProjectsUsed = (List<GetProjectUsedResponse>?)projectUsed;
                }

            }
            return results;
        }

		public async Task<bool> CheckExist(Guid schemaId)
		{
            var schema = await _schemaRepository.GetAsync(x => x.SchemaId == schemaId, null);
            if (schema == null)
            {
                return false;
            }
            return true;
		}
	}
}
