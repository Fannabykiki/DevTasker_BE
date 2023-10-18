using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
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
                SchemasId = Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990")
            };


            var newProject = await _projectRepository.CreateAsync(newProjectRequest);

			var newPO = new ProjectMember
			{
				IsOwner = true,
				MemberId = Guid.NewGuid(),
				ProjectId = newProject.ProjectId,
				UserId = newProject.CreateBy,
				RoleId = Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B")
			};

			var newAdmin = new ProjectMember
			{
				IsOwner = false,
				MemberId = Guid.NewGuid(),
				ProjectId = newProject.ProjectId,
				UserId = Guid.Parse("AFA06CDD-7713-4B81-9163-C45556E4FA4C"),
				RoleId = Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B")
			};

			await _projectMemberRepository.CreateAsync(newPO);
			await _projectMemberRepository.CreateAsync(newAdmin);
			_projectMemberRepository.SaveChanges();
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
			_roleRepository.SaveChanges();
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

	public async Task<bool> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{	
			var role = await _roleRepository.GetAsync(x => x.RoleId == updateMemberRoleRequest.RoleId,null);
			if(role == null)
			{
				return false;
			}

			var member = await _projectMemberRepository.GetAsync(x => x.MemberId == memberId, null);
			if (member == null || member.IsOwner == true || member.RoleId.Equals("5B5C81E8-722D-4801-861C-6F10C07C769B"))
			{
				return false;
			}

			member.RoleId = updateMemberRoleRequest.RoleId;
			await _projectMemberRepository.UpdateAsync(member);
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
}