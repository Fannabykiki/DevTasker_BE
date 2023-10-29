using AutoMapper;
using Capstone.Common.DTOs.Permission;
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
	private readonly ISchemaRepository _schemaRepository;
	private readonly IBoardRepository _boardRepository;
	private readonly IInterationRepository _interationRepository;
	private readonly IPermissionRepository _permissionRepository;
	private readonly IPermissionSchemaRepository _permissionScemaRepo;

	public ProjectService(CapstoneContext context, IProjectRepository projectRepository, IRoleRepository roleRepository, IMapper mapper, ISchemaRepository permissionSchemaRepository, IProjectMemberRepository projectMemberRepository, IBoardRepository boardRepository, IPermissionRepository permissionRepository, IInterationRepository interationRepository, IPermissionSchemaRepository permissionScemaRepo)
	{
		_context = context;
		_projectRepository = projectRepository;
		_roleRepository = roleRepository;
		_mapper = mapper;
		_schemaRepository = permissionSchemaRepository;
		_projectMemberRepository = projectMemberRepository;
		_boardRepository = boardRepository;
		_permissionRepository = permissionRepository;
		_interationRepository = interationRepository;
		_permissionScemaRepo = permissionScemaRepo;
	}

	public async Task<bool> CreateProject(CreateProjectRequest createProjectRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var projectId = Guid.NewGuid();

			var newProjectRequest = new Project
			{	
				ProjectId = projectId,
				ProjectName = createProjectRequest.ProjectName,
				CreateAt = createProjectRequest.CreateAt,
				EndDate = createProjectRequest.EndDate,
				StartDate = createProjectRequest.StartDate,
				PrivacyStatus = createProjectRequest.PrivacyStatus,
				ProjectStatus = StatusEnum.Active,
				CreateBy = createProjectRequest.CreateBy,
				Description = createProjectRequest.Description,
				SchemasId = Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"),
				Board = new Board
				{
					BoardId = Guid.NewGuid(),
					ProjectId = projectId,
					DeleteAt = null,
					CreateAt = DateTime.UtcNow,
					Status = StatusEnum.Active,
					Title = "Board Default",
					UpdateAt = null,
				}
			};

			var newProject = await _projectRepository.CreateAsync(newProjectRequest);
			await _projectRepository.SaveChanges();

			var newInteration = new Interation
			{
				StartDate = DateTime.UtcNow,
				Status = InterationStatusEnum.Current,
				BoardId = newProjectRequest.Board.BoardId,
				EndDate = DateTime.UtcNow.AddDays(14),
				InterationName = "Interation 1",
				InterationId = Guid.NewGuid(),
			};

			await _interationRepository.CreateAsync(newInteration);
			await _interationRepository.SaveChanges();

			var newPo = new ProjectMember
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
				RoleId = Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430")
			};

			await _projectMemberRepository.CreateAsync(newPo);
			await _projectMemberRepository.CreateAsync(newAdmin);

			await _projectMemberRepository.SaveChanges();

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

	public async Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid userId)
	{
		var projects = await _projectRepository.GetAllWithOdata(x => x.ProjectStatus == StatusEnum.Active, x => x.ProjectMembers.Where(m => m.UserId == userId));
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

			await _roleRepository.CreateAsync(newRoleRequest);
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


	public async Task<ListViewMemberProject> GetMemberByProjectId(Guid projectId)
	{
		var projects = await _projectMemberRepository.GetAllWithOdata(x => x.ProjectId == projectId, null);
		var listMem =  _mapper.Map<List<ViewMemberProject>>(projects);
		return new ListViewMemberProject {
			Members = listMem,
			numberOfMember = listMem.Count()
        };

	}

	public async Task<bool> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			await _roleRepository.GetAsync(x => x.RoleId == updateMemberRoleRequest.RoleId, null)!;

			var member = await _projectMemberRepository.GetAsync(x => x.MemberId == memberId, null)!;
			if (member.RoleId.Equals("5B5C81E8-722D-4801-861C-6F10C07C769B") || member.IsOwner == true)
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

	public async Task<bool> UpdateProjectInfo(Guid projectId, UpdateProjectNameInfo updateProjectNameInfo)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.ProjectName = updateProjectNameInfo.ProjectName;
			project.Description = updateProjectNameInfo.Description;
			await _projectRepository.UpdateAsync(project);
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

	public async Task<bool> UpdateProjectPrivacy(Guid projectId, UpdateProjectPrivacyRequest updateProjectPrivacyRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.PrivacyStatus = updateProjectPrivacyRequest.PrivacyStatus;
			await _projectRepository.UpdateAsync(project);
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

	public async Task<bool> DeleteProject(Guid projectId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.ProjectStatus = StatusEnum.Inactive;
			project.DeleteAt = DateTime.UtcNow;
			project.ExpireAt = DateTime.UtcNow.AddDays(30);
			await _projectRepository.UpdateAsync(project);
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

	public async Task<bool> RestoreProject(Guid projectId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.ProjectStatus = StatusEnum.Active;
			project.DeleteAt = null;
			project.ExpireAt = null;
			await _projectRepository.UpdateAsync(project);
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

	public async Task<GetAllProjectViewModel> GetProjectByProjectId(Guid projectId)
	{
		var projects = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
		return _mapper.Map<GetAllProjectViewModel>(projects);
	}

	public async Task<List<ViewProjectInfoRequest>> GetInfoProjectByProjectId(Guid projectId)
	{
		var projectInfoRequests = new List<ViewProjectInfoRequest>();
		var project = await _projectRepository.GetAsync(p => p.ProjectId == projectId, p => p.ProjectMembers)!;
		var projectInfoRequest = new ViewProjectInfoRequest
		{
			ProjectId = project.ProjectId,
			ProjectName = project.ProjectName,
			Description = project.Description,
			ProjectStatus = project.ProjectStatus,
			StartDate = project.StartDate,
			EndDate = project.EndDate,
			CreateBy = project.CreateBy,
			CreateAt = project.CreateAt,
			PrivacyStatus = project.PrivacyStatus,
			ProjectMembers = project.ProjectMembers
				.Select(member => new ViewMemberProject
				{
					MemberId = member.MemberId,
					UserId = member.UserId,
					RoleId = member.RoleId,
					ProjectId = member.ProjectId,
					IsOwner = member.IsOwner
				})
				.ToList()
		};

		projectInfoRequests.Add(projectInfoRequest);

		return projectInfoRequests;
	}

	public async Task<IEnumerable<PermissionViewModel>> GetPermissionByUserId(Guid projectId, Guid userId)
	{
		var newPermisisonViewModel = new List<PermissionViewModel>();
		var role = await _projectMemberRepository.GetAsync(x => x.ProjectId == projectId && x.UserId == userId, x => x.Role)!;
		var permissions = await _permissionScemaRepo.GetPermissionByUserId(role.RoleId);
		foreach (var permisison in permissions)
		{
			var permissionViewModel = new PermissionViewModel
			{
				Description = permisison.Description,
				Name = permisison.Name,
				PermissionId = permisison.PermissionId,
			};
			newPermisisonViewModel.Add(permissionViewModel);
		}
		return newPermisisonViewModel;
	}
}