using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.Common.Enums;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.Project;

public class ProjectService : IProjectService
{
    private readonly CapstoneContext _context;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionSchemaRepository _permissionSchemaRepository;

	public ProjectService(CapstoneContext context, IProjectRepository projectRepository, IRoleRepository roleRepository, IMapper mapper, IPermissionSchemaRepository permissionSchemaRepository)
	{
		_context = context;
		_projectRepository = projectRepository;
		_roleRepository = roleRepository;
		_mapper = mapper;
		_permissionSchemaRepository = permissionSchemaRepository;
	}

	public async Task<bool> CreateProject(CreateProjectRequest createProjectRequest)
    {
        using var transaction = _projectRepository.DatabaseTransaction();
        try
        {
            var newProjectRequest = new DataAccess.Entities.Project
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = createProjectRequest.ProjectName,
                CreateAt = createProjectRequest.CreateAt,
                EndDate = createProjectRequest.EndDate,
                StartDate = createProjectRequest.StartDate,
                PrivacyStatus = true,
                ProjectStatus = StatusEnum.Active,
                CreateBy = createProjectRequest.CreateBy
            };

            await _projectRepository.CreateAsync(newProjectRequest);
            _projectRepository.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<IEnumerable<GetAllProjectViewModel>> GetAllProjects(Guid UserId)
    {
        var projects = await _projectRepository.GetAllWithOdata(x => true, x => x.ProjectMembers.Where(x=> x.UserId == UserId));
        return _mapper.Map<List<GetAllProjectViewModel>>(projects);
    }

	public async Task<bool> CreateProjectRole(CreateRoleRequest createRoleRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var newRoleRequest = new Role
			{
				RoleId = Guid.NewGuid(),
                RoleName = createRoleRequest.RoleName,
                Description = createRoleRequest.Description
			};

			var newRole = await _roleRepository.CreateAsync(newRoleRequest);

            foreach (var PermissionId in createRoleRequest.PermissionId)
            {
                var newSchema = new PermissionSchema
                {
                    PermissionId = PermissionId,
                    Description = createRoleRequest.SchemaDes,
                    SchemaName = createRoleRequest.SchemaName,
                    RoleId = newRole.RoleId
                };
				await _permissionSchemaRepository.CreateAsync(newSchema);
			}
			_permissionSchemaRepository.SaveChanges();
			_projectRepository.SaveChanges();
			transaction.Commit();

			return true;
		}
		catch (Exception)
		{
			transaction.RollBack();
			return false;
		}
	}
}