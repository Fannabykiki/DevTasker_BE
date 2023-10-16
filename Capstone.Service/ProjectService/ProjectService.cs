using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.Common.Enums;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.ProjectService;

public class ProjectService : IProjectService
{
    private readonly CapstoneContext _context;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IPermissionSchemaRepository _permissionSchemaRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IPermissionRepository _permissionRepository;

	public ProjectService(CapstoneContext context, IProjectRepository projectRepository, IRoleRepository roleRepository, IMapper mapper, IPermissionSchemaRepository permissionSchemaRepository, IProjectMemberRepository projectMemberRepository, IBoardRepository boardRepository, IPermissionRepository permission, IPermissionRepository permissionRepository)
	{
		_context = context;
		_projectRepository = projectRepository;
		_roleRepository = roleRepository;
		_mapper = mapper;
		_permissionSchemaRepository = permissionSchemaRepository;
		_projectMemberRepository = projectMemberRepository;
		_boardRepository = boardRepository;
		_permissionRepository = permissionRepository;
	}

	public async Task<bool> CreateProject(CreateProjectRequest createProjectRequest)
    {
        using var transaction = _projectRepository.DatabaseTransaction();
        try
        {
            var newProjectRequest = new Project
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = createProjectRequest.ProjectName,
                CreateAt = createProjectRequest.CreateAt,
                EndDate = createProjectRequest.EndDate,
                StartDate = createProjectRequest.StartDate,
                PrivacyStatus = createProjectRequest.PrivacyStatus,
                ProjectStatus = StatusEnum.Active,
                CreateBy = createProjectRequest.CreateBy,
                Description = createProjectRequest.Description,
            };

            var newBoard = new Board
            {
                BoardId = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow,
                ProjectId = newProjectRequest.ProjectId,
                Title = "",
            };
            await _boardRepository.CreateAsync(newBoard);
            _boardRepository.SaveChanges();
            var newProject = await _projectRepository.CreateAsync(newProjectRequest);

            var newPORole = new Role()
            {
	            RoleId = Guid.NewGuid(),
				RoleName = "Project Owner",
				Description = "This project belong to him/her",
				ProjectId = newProject.ProjectId
            };
            
            var newAdminRole = new Role()
            {
	            RoleId = Guid.NewGuid(),
	            RoleName = "Project Owner",
	            Description = "Devtasker admin",
	            ProjectId = newProject.ProjectId
            };
           
            var permissions = await _permissionRepository.GetAllWithOdata(x => true, null);
			
            foreach (var Permisison in permissions)
            {
	            var newSchemas = new PermissionSchema()
	            {
		            Description = "Admin of project",
		            RoleId = newPORole.RoleId,
		            SchemaName = "Admin schemas",
		            PermissionId = Permisison.PermissionId
	            };
            }
            
            var newPo = new ProjectMember
            {
                IsOwner = true,
                MemberId = Guid.NewGuid(),
                ProjectId = newProject.ProjectId,
                UserId = newProject.CreateBy,
                RoleId = newPORole.RoleId
            };

			var admin = new ProjectMember
			{
				IsOwner = false,
				MemberId = Guid.NewGuid(),
				ProjectId = newProject.ProjectId,
				UserId = Guid.Parse("afa06cdd77134b819163c45556e4fa4c"),
				RoleId = newAdminRole.RoleId
			};
			
			
			
            await _projectMemberRepository.CreateAsync(admin);
			await _projectMemberRepository.CreateAsync(newPo);

			_projectRepository.SaveChanges();
			_projectMemberRepository.SaveChanges();

            transaction.Commit();
            return true;
        }
        catch (Exception)
        {
            transaction.RollBack();
            return false;
        }
    }

    public async Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid UserId)
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

	public async Task<IEnumerable<ViewMemberProject>> GetMemberByProjectId(Guid projectId)
	{
		var projects = await _projectMemberRepository.GetAllWithOdata(x => x.ProjectId == projectId, null);
		return _mapper.Map<List<ViewMemberProject>>(projects);
	}
}