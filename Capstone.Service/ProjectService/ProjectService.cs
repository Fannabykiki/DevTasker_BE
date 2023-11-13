
using AutoMapper;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Capstone.Service.ProjectService;

public class ProjectService : IProjectService
{
	private readonly CapstoneContext _context;
	private readonly IProjectRepository _projectRepository;
	private readonly IMapper _mapper;
	private readonly IRoleRepository _roleRepository;
	private readonly IStatusRepository _statusRepository;
	private readonly IProjectMemberRepository _projectMemberRepository;
	private readonly ISchemaRepository _schemaRepository;
	private readonly IBoardRepository _boardRepository;
	private readonly IInterationRepository _interationRepository;
	private readonly IPermissionRepository _permissionRepository;
	private readonly IPermissionSchemaRepository _permissionScemaRepo;
	private readonly IBoardStatusRepository _boardStatusRepository;
	private readonly IUserRepository _userRepository;
	private readonly ITaskTypeRepository _ticketTypeRepository;
	private readonly IPriorityRepository _priorityRepository;
	private readonly ITaskRepository _ticketRepository;

	public ProjectService(CapstoneContext context, IProjectRepository projectRepository, IRoleRepository roleRepository, IMapper mapper, ISchemaRepository permissionSchemaRepository, IProjectMemberRepository projectMemberRepository, IBoardRepository boardRepository, IPermissionRepository permissionRepository, IInterationRepository interationRepository, IPermissionSchemaRepository permissionScemaRepo, IStatusRepository statusRepository, IBoardStatusRepository boardStatusRepository, IUserRepository userRepository, ITaskTypeRepository ticketTypeRepository, IPriorityRepository priorityRepository, ITaskRepository ticketRepository)
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
		_statusRepository = statusRepository;
		_boardStatusRepository = boardStatusRepository;
		_userRepository = userRepository;
		_ticketTypeRepository= ticketTypeRepository;
		_priorityRepository = priorityRepository;
		_ticketRepository= ticketRepository;
    }

	public async Task<CreateProjectRespone> CreateProject(CreateProjectRequest createProjectRequest, Guid userId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var projectId = Guid.NewGuid();

			var newProjectRequest = new Project
			{
				ProjectId = projectId,
				ProjectName = createProjectRequest.ProjectName,
				CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
				EndDate = DateTime.Parse(createProjectRequest.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
				StartDate = DateTime.Parse(createProjectRequest.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
				PrivacyStatus = createProjectRequest.PrivacyStatus,
				StatusId = Guid.Parse("53F76F08-FF3C-43EB-9FF4-C9E028E513D5"),
				CreateBy = userId,
				IsDelete = false,
				Description = createProjectRequest.Description,
				SchemasId = Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"),
				Board = new Board
				{
					BoardId = Guid.NewGuid(),
					ProjectId = projectId,
					DeleteAt = null,
					CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
					StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE"),
					Title = "Board Default",
					UpdateAt = null,
				}
			};

			var newProject = await _projectRepository.CreateAsync(newProjectRequest);
			await _projectRepository.SaveChanges();

			var todo = new BoardStatus
			{
				BoardId = newProject.Board.BoardId,
				Title = "To do",
				Order = 1,
				BoardStatusId = new Guid()
			};

			var inProgress = new BoardStatus
			{
				BoardId = newProject.Board.BoardId,
				Title = "In Progress",
				Order = 2,
				BoardStatusId = new Guid()
			};
			
			var done = new BoardStatus
			{
				BoardId = newProject.Board.BoardId,
				Title = "Done",
				Order = 3,
				BoardStatusId = new Guid()
			};

			await _boardStatusRepository.CreateAsync(done);
			await _boardStatusRepository.CreateAsync(todo);
			await _boardStatusRepository.CreateAsync(inProgress);
			await _boardStatusRepository.SaveChanges();
			
			var newInteration = new Interation
			{
				StartDate = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
				StatusId = Guid.Parse("3FC7B979-BC37-4E06-B38A-B01245541867"),
				BoardId = newProject.Board.BoardId,
				EndDate = DateTime.Parse(DateTime.UtcNow.AddDays(14).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
				InterationName = "Interation 1",
				InterationId = Guid.NewGuid(),
			};

			 var inter = await _interationRepository.CreateAsync(newInteration);
			await _interationRepository.SaveChanges();

			var newPo = new ProjectMember
			{
				IsOwner = true,
				MemberId = Guid.NewGuid(),
				ProjectId = newProject.ProjectId,
				UserId = newProject.CreateBy,
				RoleId = Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B"),
				StatusId = Guid.Parse("ba888147-c90a-4578-8ba6-63ba1756fac1")
			};

			var newAdmin = new ProjectMember
			{
				IsOwner = false,
				MemberId = Guid.NewGuid(),
				ProjectId = newProject.ProjectId,
				UserId = Guid.Parse("AFA06CDD-7713-4B81-9163-C45556E4FA4C"),
				RoleId = Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430"),
				StatusId = Guid.Parse("ba888147-c90a-4578-8ba6-63ba1756fac1")
			};

			await _projectMemberRepository.CreateAsync(newPo);
			await _projectMemberRepository.CreateAsync(newAdmin);

			await _projectMemberRepository.SaveChanges();

			transaction.Commit();

			return new CreateProjectRespone
			{
				CreateAt = newProject.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
				CreateBy = newProject.CreateBy,
				DeleteAt = newProject.DeleteAt,
				Description = newProject.Description,
				StartDate = newProject.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
				EndDate = newProject.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
				PrivacyStatus = newProject.PrivacyStatus,
				ProjectId = newProject.ProjectId,
				ProjectName = newProject.ProjectName,
				SchemasId = newProject.SchemasId,
				StatusId = newProject.StatusId,
				BoardId = newProject.Board.BoardId,
				InterationId = inter.InterationId,
				BaseResponse = new Common.DTOs.Base.BaseResponse
				{
					IsSucceed = true,
					Message = "Create successfully"
				}
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error occurred: " + ex.Message);
			transaction.RollBack();
			return new CreateProjectRespone
			{
				BaseResponse = new Common.DTOs.Base.BaseResponse
				{
					IsSucceed = false,
					Message = "Create successfully"
				}
			};
		}
	}

	public async Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid userId)
	{
		var projects = await _projectMemberRepository.GetAllWithOdata(x => x.UserId == userId, x => x.Project);
		return _mapper.Map<List<GetAllProjectViewModel>>(projects);
	}
	public async Task<IEnumerable<GetUserProjectAnalyzeResponse>> GetUserProjectAnalyze(Guid userId)
	{
		var listProjectAnalyze = new List<GetUserProjectAnalyzeResponse>();
		var projects = await _projectRepository.GetAllWithOdata(x => true, x => x.ProjectMembers.Where(m => m.UserId == userId));
		foreach (var project in projects)
		{
			var manager = await _projectMemberRepository.GetAsync(x => x.ProjectId == project.ProjectId && x.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B"), x => x.Users);
			var projectStatus = await _statusRepository.GetAsync(x => x.StatusId == project.StatusId, x => x.Users);
			var projectAnalyze = new GetUserProjectAnalyzeResponse();
			projectAnalyze.ProjectId = project.ProjectId;
			projectAnalyze.ProjectName = project.ProjectName;
			projectAnalyze.ProjectStatus = projectStatus.Title;
			projectAnalyze.Manager = new UserResponse
			{
				UserId = manager.UserId,
				UserName = manager.Users.Fullname,
				Email = manager.Users.Email,
				PhoneNumber = manager.Users.PhoneNumber,
				Dob = manager.Users.Dob,
				IsAdmin = manager.Users.IsAdmin,
			};
			listProjectAnalyze.Add(projectAnalyze);
		}
		return listProjectAnalyze;
	}

	public async Task<BaseResponse> CreateProjectRole(CreateRoleRequest createRoleRequest)
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
			await _roleRepository.SaveChanges();

			transaction.Commit();
			return new BaseResponse { IsSucceed = true, Message = "Create Project Role successfully" };
		}
		catch (Exception)
		{
			transaction.RollBack();
			return new BaseResponse { IsSucceed = false, Message = "Create Project Role fail" };
		}
	}


	public async Task<IEnumerable<ViewMemberProject>> GetMemberByProjectId(Guid projectId)
	{
		var projects = await _projectMemberRepository.GetAllWithOdata(x => x.ProjectId == projectId, null);
		return _mapper.Map<List<ViewMemberProject>>(projects);
	}

	public async Task<BaseResponse> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			await _roleRepository.GetAsync(x => x.RoleId == updateMemberRoleRequest.RoleId, null)!;

			var member = await _projectMemberRepository.GetAsync(x => x.MemberId == memberId, null)!;
			if (member.RoleId.Equals("5B5C81E8-722D-4801-861C-6F10C07C769B") || member.IsOwner == true)
			{
				return new BaseResponse { IsSucceed = false, Message = "Update Member Role successfully" };
			}

			member.RoleId = updateMemberRoleRequest.RoleId;
			await _projectMemberRepository.UpdateAsync(member);
			await _projectMemberRepository.SaveChanges();

			transaction.Commit();

			return new BaseResponse { IsSucceed = true, Message = "Update Member Role successfully" };
		}
		catch (Exception)
		{
			transaction.RollBack();
			return new BaseResponse { IsSucceed = false, Message = "Update Member Role fail" };
		}
	}

	public async Task<BaseResponse> UpdateProjectInfo(Guid projectId, UpdateProjectNameInfo updateProjectNameInfo)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.ProjectName = updateProjectNameInfo.ProjectName;
			project.Description = updateProjectNameInfo.Description;
			await _projectRepository.UpdateAsync(project);
			await _projectRepository.SaveChanges();
			transaction.Commit();
			return new BaseResponse { IsSucceed = true, Message = "Update Project Info successfully" };
		}
		catch (Exception)
		{
			transaction.RollBack();
			return new BaseResponse { IsSucceed = false, Message = "Update Project Privacy fail" };
		}
	}

	public async Task<BaseResponse> UpdateProjectPrivacy(Guid projectId, UpdateProjectPrivacyRequest updateProjectPrivacyRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.PrivacyStatus = updateProjectPrivacyRequest.PrivacyStatus;
			await _projectRepository.UpdateAsync(project);
			await _projectRepository.SaveChanges();
			transaction.Commit();
			return new BaseResponse { IsSucceed = true, Message = "Update Project Privacy successfully" };
		}
		catch (Exception)
		{
			transaction.RollBack();
			return new BaseResponse { IsSucceed = false, Message = "Update Project Privacy fail" };
		}
	}

	public async Task<BaseResponse> DeleteProject(Guid projectId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.StatusId = Guid.Parse("DB6CBA9F-6B55-4E18-BBC1-624AFDCD92C7");
			project.DeleteAt = DateTime.UtcNow;
			project.ExpireAt = DateTime.UtcNow.AddDays(30);
			await _projectRepository.UpdateAsync(project);
			await _projectRepository.SaveChanges();
			transaction.Commit();
			return new BaseResponse { IsSucceed = true, Message = "Delete successfully" };
		}
		catch (Exception)
		{
			transaction.RollBack();
			return new BaseResponse { IsSucceed = false, Message = "Delete fail" };
		}
	}

	public async Task<BaseResponse> RestoreProject(Guid projectId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
			project.StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE");
			project.DeleteAt = null;
			project.ExpireAt = null;
			await _projectRepository.UpdateAsync(project);
			await _projectRepository.SaveChanges();
			transaction.Commit();
			return new BaseResponse
			{
				IsSucceed = true,
				Message = "Restore successfully"
			};
		}
		catch (Exception)
		{
			transaction.RollBack();
			return new BaseResponse
			{
				IsSucceed = false,
				Message = "Restore fail"
			};
		}
	}

	public async Task<GetAllProjectViewModel> GetProjectByProjectId(Guid projectId)
	{
		var projects = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null)!;
		return _mapper.Map<GetAllProjectViewModel>(projects);
	}

	public async Task<ViewProjectInfoRequest> GetInfoProjectByProjectId(Guid projectId)
	{
		var projectInfoRequests = new ViewProjectInfoRequest();
		var project = await _projectRepository.GetAsync(p => p.ProjectId == projectId, p => p.Status)!;
		var members = await _projectMemberRepository.GetAllWithOdata(m => m.ProjectId == projectId, p => p.Role)!;
		projectInfoRequests = new ViewProjectInfoRequest
		{
			ProjectId = project.ProjectId,
			ProjectName = project.ProjectName,
			Description = project.Description,
			StatusId = project.StatusId,
			StartDate = project.StartDate,
			EndDate = project.EndDate,
			CreateBy = project.CreateBy,
			CreateAt = project.CreateAt,
			ProjectStatus = project.Status.Title,
			PrivacyStatus = project.PrivacyStatus,
			ProjectMembers = members
				.Select(m => new ViewMemberProject
				{
					MemberId = m.MemberId,
					UserId = m.UserId,
					RoleId = m.RoleId,
					ProjectId = m.ProjectId,
					IsOwner = m.IsOwner,
					RoleName = m.Role.RoleName
				})
				.ToList()
		};
        foreach (var mem in projectInfoRequests.ProjectMembers)
        {
            var member = await _projectMemberRepository.GetAsync(x => x.ProjectId == projectId, x => x.Users);
            mem.Fullname = member.Users.Fullname;
            mem.Email = member.Users.Email;
        }
        return projectInfoRequests;
	}

	public async Task<bool?> SendMailInviteUser(InviteUserRequest inviteUserRequest)
	{
		foreach (var user in inviteUserRequest.Email)
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == inviteUserRequest.ProjectId, null);
			string verificationLink = "https://devtasker.azurewebsites.net/invitation?" + "email=" + user + "&projectId=" + inviteUserRequest.ProjectId;
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("devtaskercapstone@gmail.com"));
			email.To.Add(MailboxAddress.Parse("" + user));
			email.Subject = "DevTakser verification step";
			email.Body = new TextPart(TextFormat.Html)
			{
				Text = $"<h1>You've been invited to DevTasker</h1>" +
				$"<h2>Project Name: {project.ProjectName} </h2><p>Click the link below to accept invitation</p><a href=\"{verificationLink}\">Join now</a>"
			};

			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				client.Authenticate("devtaskercapstone@gmail.com", "fbacmmlfxlmchkmc");
				client.Send(email);
				client.Disconnect(true);
			}
		}
		return true;

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

	public async Task<IQueryable<GetAllProjectResponse>> GetProjectsAdmin()
	{
		var projects = await _projectRepository.GetAllWithOdata(x => true, x => x.Status);
		var projectsList = new List<GetAllProjectResponse>();
		foreach (var project in projects)
		{
			var members = await _projectMemberRepository.GetAllWithOdata(x => x.ProjectId == project.ProjectId, x => x.Users.Status);
			UserResponse manager = new UserResponse();
			List<UserResponse> listMember = new List<UserResponse>();
			foreach (var member in members)
			{
				if (member.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B"))
				{
					manager = new UserResponse
					{
						UserId = member.UserId,
						UserName = member.Users.Fullname,
						Email = member.Users.Email,
						IsAdmin = member.Users.IsAdmin,
						StatusName = member.Users.Status.Title,
					};
				}
				else
				{
					listMember.Add(new UserResponse
					{
						UserId = member.UserId,
						UserName = member.Users.Fullname,
						Email = member.Users.Email,
						IsAdmin = member.Users.IsAdmin,
						StatusName = member.Users.Status.Title,
					});
				}
			}

			if (listMember.Count() == 0) listMember = null;
			if (manager == null) manager = null;

			projectsList.Add(new GetAllProjectResponse
			{
				ProjectId = project.ProjectId,
				ProjectName = project.ProjectName,
				Description = project.Description,
				ProjectStatus = project.Status.Title,
				StartDate = project.StartDate,
				EndDate = project.EndDate,
				CreateAt = project.CreateAt,
				DeleteAt = project.DeleteAt,
				Manager = manager,
				Member = listMember,
				ExpireAt = project.ExpireAt,
				PrivacyStatus = project.PrivacyStatus,
			});
		}
		var respone = projectsList.AsQueryable();
		return respone;
	}


	public async Task<ProjectAnalyzeRespone> ProjectAnalyzeAdmin()
	{
		var projects = await _projectRepository.GetAllWithOdata(x => true, x => x.Status);
		var totalProject = projects.Count();
		var activeProject = projects.Where(x => x.StatusId == Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE")).Count();
		var inactiveProject = projects.Where(x => x.StatusId == Guid.Parse("DB6CBA9F-6B55-4E18-BBC1-624AFDCD92C7")).Count();
		var deleteProject = totalProject - activeProject - inactiveProject;
		var activeProjectPercent = (int)Math.Round((double)(100 * activeProject) / totalProject);
		var inactiveProjectPercent = (int)Math.Round((double)(100 * inactiveProject) / totalProject);
		var deleteProjectPercent = 100 - activeProjectPercent - inactiveProjectPercent;

		return new ProjectAnalyzeRespone
		{
			TotalProject = totalProject,
			ProjectActive = activeProject,
			ProjectActivePercent = activeProjectPercent,
			ProjectDelete = deleteProject,
			ProjectDeletePercent = deleteProjectPercent,
			ProjectInActive = inactiveProject,
			ProjectInActivePercent = inactiveProjectPercent,
		};
	}

    public async Task<List<GetProjectTasksResponse>> GetProjectsTasks(Guid projectId)
    {
        var results = new List<GetProjectTasksResponse>();
        var iterations = await _interationRepository.GetAllWithOdata(x => x.BoardId == projectId, null);
        if (iterations == null) return null;
		foreach ( var interation in iterations)
		{
			var tasks = await _ticketRepository.GetAllWithOdata(x => x.InterationId == interation.InterationId,x => x.Status);
			foreach ( var task in tasks)
			{
				var assignTo = await _userRepository.GetAsync(x => x.UserId == task.AssignTo,null);
				var createBy = await _userRepository.GetAsync(x => x.UserId == task.CreateBy, null);
				var taskType = await _ticketTypeRepository.GetAsync(x => x.TypeId == task.TypeId,null);
				var priority = await _priorityRepository.GetAsync(x => x.LevelId == task.PriorityId,null);

                var newTask = new GetProjectTasksResponse();
				newTask.TaskId = task.TaskId;
				newTask.Title = task.Title;
				newTask.Decription = task.Decription;
				newTask.StartDate= task.StartDate;
				newTask.DueDate = task.DueDate;
				newTask.CreateTime= task.CreateTime;
				newTask.DeleteAt = task.DeleteAt;
				newTask.AssignTo = _mapper.Map<UserResponse>(assignTo); 
				newTask.CreateBy = _mapper.Map<UserResponse>(createBy);
				newTask.TaskType = taskType.Title;
				newTask.PrevId= task.PrevId;
				newTask.StatusId= task.StatusId;
				newTask.TaskStatus = task.Status.Title;
				newTask.Priority = priority.Title;
				newTask.Interation = interation.InterationName;
                results.Add(newTask);
            }
		}
		return results;
    }

	public async Task<List<InterationViewModel>> GetInterationByProjectId(Guid projectId)
	{
		var results = await _interationRepository.GetAllWithOdata(x => x.BoardId == projectId, x => x.Status);
		return _mapper.Map<List<InterationViewModel>>(results);
	}
}