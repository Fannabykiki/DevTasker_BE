
using AutoMapper;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Invitaion;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.Hubs;
using Google.Apis.Drive.v3.Data;
using MailKit.Security;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
	private readonly IPermissionSchemaRepository _permissionSchemaRepository;
	private readonly IBoardStatusRepository _boardStatusRepository;
	private readonly IUserRepository _userRepository;
	private readonly ITaskTypeRepository _ticketTypeRepository;
	private readonly IPriorityRepository _priorityRepository;
	private readonly ITaskRepository _ticketRepository;
	private readonly IInvitationRepository _invitationRepository;

    private readonly PresenceTracker _presenceTracker;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;
    public ProjectService(CapstoneContext context, 
		IProjectRepository projectRepository, 
		IRoleRepository roleRepository, 
		IMapper mapper, 
		ISchemaRepository permissionSchemaRepository, 
		IProjectMemberRepository projectMemberRepository, 
		IBoardRepository boardRepository, 
		IPermissionRepository permissionRepository, 
		IInterationRepository interationRepository, 
		IPermissionSchemaRepository permissionScemaRepo,
		IStatusRepository statusRepository,
		IBoardStatusRepository boardStatusRepository,
		IUserRepository userRepository,
		ITaskTypeRepository ticketTypeRepository,
		IPriorityRepository priorityRepository, 
		ITaskRepository ticketRepository, 
		IInvitationRepository invitationRepository,
        PresenceTracker presenceTracker,
        IHubContext<NotificationHub> hubContext,
        INotificationRepository notificationRepository
        )
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
		_permissionSchemaRepository = permissionScemaRepo;
		_statusRepository = statusRepository;
		_boardStatusRepository = boardStatusRepository;
		_userRepository = userRepository;
		_ticketTypeRepository = ticketTypeRepository;
		_priorityRepository = priorityRepository;
		_ticketRepository = ticketRepository;
		_invitationRepository = invitationRepository;
		_presenceTracker = presenceTracker;
		_hubContext = hubContext;
		_notificationRepository = notificationRepository;
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
				BoardStatusId = new Guid(),
				StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE")
			};

			var inProgress = new BoardStatus
			{
				BoardId = newProject.Board.BoardId,
				Title = "In Progress",
				Order = 2,
				BoardStatusId = new Guid(),
				StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE")
			};

			var done = new BoardStatus
			{
				BoardId = newProject.Board.BoardId,
				Title = "Done",
				Order = 3,
				BoardStatusId = new Guid(),
				StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE")
			};

			var fail = new BoardStatus
			{
				BoardId = newProject.Board.BoardId,
				Title = "Fail",
				Order = 4,
				BoardStatusId = new Guid(),
				StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE")
			};

			await _boardStatusRepository.CreateAsync(done);
			await _boardStatusRepository.CreateAsync(todo);
			await _boardStatusRepository.CreateAsync(inProgress);
			await _boardStatusRepository.CreateAsync(fail);
			await _boardStatusRepository.SaveChanges();

			var newInteration = new Interation
			{
				StartDate = DateTime.Parse(newProject.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
				StatusId = Guid.Parse("3FC7B979-BC37-4E06-B38A-B01245541867"),
				BoardId = newProject.Board.BoardId,
				EndDate = DateTime.Parse(newProject.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
				InterationName = "Sprint 1",
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
					Message = "Create fail"
				}
			};
		}
	}

	public async Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid userId)
	{
		var projects = await _projectMemberRepository.GetProjectByUserId(userId);
		return _mapper.Map<List<GetAllProjectViewModel>>(projects);
	}
	public async Task<IEnumerable<GetUserProjectAnalyzeResponse>> GetUserProjectAnalyze(Guid userId)
	{
		var allProjects = await _projectMemberRepository.GetAllWithOdata(x => x.UserId == userId, x => x.Project);

		HashSet<GetUserProjectAnalyzeResponse> projectResult = new HashSet<GetUserProjectAnalyzeResponse>();
		HashSet<Guid> projectIds = new HashSet<Guid>();
		foreach (var record in allProjects)
		{
			projectIds.Add(record.ProjectId);
		}

		foreach (var projectId in projectIds)
		{
			var projects = await _projectRepository.GetAsync(x => x.ProjectId == projectId, x => x.Status);
			var manager = await _projectMemberRepository.GetAsync(x => x.ProjectId == projectId && x.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B"), x => x.Users);
			manager.Users.Status = await _statusRepository.GetAsync(x => x.StatusId == manager.Users.StatusId, null);
			var projectAnalyze = new GetUserProjectAnalyzeResponse();
			projectAnalyze.ProjectId = projects.ProjectId;
			projectAnalyze.ProjectName = projects.ProjectName;
			projectAnalyze.ProjectStatus = projects.Status.Title;
			projectAnalyze.StartDate = projects.StartDate;
			projectAnalyze.Manager = new UserResponse
			{
				UserId = manager.UserId,
				UserName = manager.Users.Fullname,
				Email = manager.Users.Email,
				PhoneNumber = manager.Users.PhoneNumber,
				Dob = manager.Users.Dob,
				IsAdmin = manager.Users.IsAdmin,
				Address = manager.Users.Address,
				StatusName = manager.Users.Status.Title,
			};
			projectResult.Add(projectAnalyze);

		}
		return projectResult.ToList();
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
		var projects = await _projectMemberRepository.GetAllProjectMember(projectId);
		return _mapper.Map<List<ViewMemberProject>>(projects);
	}

	public async Task<BaseResponse> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest, Guid updateBy)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			await _roleRepository.GetAsync(x => x.RoleId == updateMemberRoleRequest.RoleId, null)!;

			var member = await _projectMemberRepository.GetAsync(x => x.MemberId == memberId, x=>x.Status)!;

			if(member.StatusId == Guid.Parse("2D79988F-49C8-4BF4-B5AB-623559B30746") || member.StatusId == Guid.Parse("A29BF1E9-2DE2-4E5F-A6DA-32D88FCCD274"))
			{
				return null;
			}

            if (member.RoleId == Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430"))
            {
                return new BaseResponse { IsSucceed = false, Message = "You cannot change the role of the user who has the System Admin role" };
            }

            if (updateMemberRoleRequest.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B"))
            {
                if (member.RoleId == Guid.Parse("5B5C81E8-722D-4801-861C-6F10C07C769B") && member.IsOwner == true)
                {
                    return new BaseResponse { IsSucceed = true, Message = "Update Member Role successfully" };
                }
				
                var updateByUser = await _projectMemberRepository.GetAsync(x => x.ProjectId == member.ProjectId && x.UserId == updateBy, null)!;
				if (updateByUser.RoleId == Guid.Parse("7ACED6BC-0B25-4184-8062-A29ED7D4E430"))
				{
					var PO = await _projectMemberRepository.GetAsync(x => x.ProjectId == member.ProjectId && x.IsOwner == true, null)!;
                    PO.IsOwner = false;
                    PO.RoleId = Guid.Parse("0A0994FC-CBAE-482F-B5E8-160BB8DDCD56");
                    await _projectMemberRepository.UpdateAsync(PO);
                    await _projectMemberRepository.SaveChanges();
                }
				else
				{
                    updateByUser.IsOwner = false;
                    updateByUser.RoleId = Guid.Parse("0A0994FC-CBAE-482F-B5E8-160BB8DDCD56");
                    await _projectMemberRepository.UpdateAsync(updateByUser);
                    await _projectMemberRepository.SaveChanges();
                }
                member.IsOwner = true;
                
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
			project.EndDate = updateProjectNameInfo.EndDate;
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
			project.StatusId = Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31");
			project.DeleteAt = DateTime.Now;
			project.ExpireAt = DateTime.Now.AddDays(30);
			project.IsDelete = true;

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
			project.StatusId = Guid.Parse("53F76F08-FF3C-43EB-9FF4-C9E028E513D5");

			project.DeleteAt = null;
			project.ExpireAt = null;
			project.IsDelete = false;

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

	public async Task<GetAllProjectViewModel> GetProjectByProjectId(Guid? projectId)
	{
		var projects = await _projectRepository.GetAsync(x => x.ProjectId == projectId, x=>x.Status)!;
		return _mapper.Map<GetAllProjectViewModel>(projects);
	}

	public async Task<ViewProjectInfoRequest> GetInfoProjectByProjectId(Guid projectId)
	{
		var projectInfoRequests = new ViewProjectInfoRequest();
		var project = await _projectRepository.GetAsync(p => p.ProjectId == projectId, p => p.Status)!;
		var members = await _projectMemberRepository.GetProjectMembers(projectId)!;
		var boardStatus = await _boardStatusRepository.GetAllWithOdata(x => x.BoardId == project.ProjectId, null);
		var totaltaskCompleted = 10;
		foreach (var item in boardStatus)
		{
			if (item.Title.Equals("Done"))
			{
				totaltaskCompleted = (await _ticketRepository.GetAllTaskCompleted(projectId, item.BoardStatusId)).Count();
			}
		}
		var totaltask = await _ticketRepository.GetAllTask(projectId);
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
			BoardId = project.ProjectId,
			ProjectStatus = project.Status.Title,
			PrivacyStatus = project.PrivacyStatus,
			TotalTaskCreated = totaltask.Count(),
			TotalTaskCompleted = totaltaskCompleted,
			ProjectMembers = members
				.Select(m => new ViewMemberProject
				{
					MemberId = m.MemberId,
					UserId = m.UserId,
					RoleId = m.RoleId,
					ProjectId = m.ProjectId,
					IsOwner = m.IsOwner,
					RoleName = m.Role.RoleName,
					UserName = m.Users.UserName,
					Email = m.Users.Email,
					Fullname = m.Users.Fullname,
					StatusId = m.StatusId,
					StatusName = m.Status.Title
				})
				.ToList()
		};
		return projectInfoRequests;
	}

	public async Task<bool?> SendMailInviteUser(InviteUserRequest inviteUserRequest, Guid userId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == inviteUserRequest.ProjectId, x=>x.ProjectMembers);
			var member = await _projectMemberRepository.GetAsync(x => x.ProjectId == inviteUserRequest.ProjectId && x.UserId == userId, null);
			var actionUser = await _userRepository.GetAsync(x => x.UserId == userId, null);
			foreach (var user in inviteUserRequest.Email)
			{
				var newInvite = new Invitation
				{
					CreateAt = DateTime.Now,
					CreateBy = member.MemberId,
					InvitationId = Guid.NewGuid(),
					InviteTo = user,
					ProjectName = project.ProjectName,
					ProjectId = project.ProjectId,
					StatusId = Guid.Parse("2D79988F-49C8-4BF4-B5AB-623559B30746"),
				};
				var invitation = await _invitationRepository.CreateAsync(newInvite);
				await _invitationRepository.SaveChanges();
				transaction.Commit();

				string verificationLink = "https://devtasker.azurewebsites.net/invitation?" + "invitation=" + invitation.InvitationId;
				var email = new MimeMessage();
				email.From.Add(MailboxAddress.Parse("devtaskercapstone@gmail.com"));
				email.To.Add(MailboxAddress.Parse("" + user));
				email.Subject = "DevTakser verification step";
				email.Body = new TextPart(TextFormat.Html)
				{
					Text = $"<h1>You've been invited to DevTasker</h1>" 
					+ $"<h2>Project Name: {project.ProjectName} </h2><p>Click the link below to accept invitation</p><a href=\"{verificationLink}\">Join now</a>"
				};

				using (var client = new MailKit.Net.Smtp.SmtpClient())
				{
					client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
					client.Authenticate("devtaskercapstone@gmail.com", "fbacmmlfxlmchkmc");
					client.Send(email);
					client.Disconnect(true);
				}
				var receiver = await _userRepository.GetAsync(x => x.Email == user, null);
				if (receiver == null) continue;
                await SendNotificationForInvitation(project.ProjectName, actionUser.UserName, receiver.UserId, verificationLink);
			}
		}
		catch
		{
			transaction.RollBack();
			return false;
		}
		return true;
	}
	public async System.Threading.Tasks.Task SendNotificationForInvitation(string projectName, string userName, Guid receiverId, string verificationLink)
	{
		var notification = new Notification
		{
			NotificationId = Guid.NewGuid(),
			Title = "Invite to project",
			Description = $"User <strong>{userName}</strong> invited you to project <strong>{projectName}</strong>",
			CreateAt = DateTime.Now,
			IsRead = false,
			RecerverId = receiverId,
			TargetUrl = verificationLink,
		};
		await _notificationRepository.CreateAsync(notification);
		await _notificationRepository.SaveChanges();

        if (await _presenceTracker.IsOnlineUser(receiverId.ToString()))
        {
            await _hubContext.Clients.Group(receiverId.ToString()).SendAsync("EmitNotification");
        }
    }


    public async Task<IEnumerable<PermissionViewModel>> GetPermissionByUserId(Guid projectId, Guid userId)
	{
		var newPermisisonViewModel = new List<PermissionViewModel>();
		var role = await _projectMemberRepository.GetAsync(x => x.ProjectId == projectId && x.UserId == userId, x => x.Role)!;
		var permissions = await _permissionSchemaRepository.GetPermissionByUserId(role.RoleId);
		HashSet<Guid> result = new HashSet<Guid>();
		foreach (var permission in permissions)
		{
			result.Add(permission.PermissionId);
		}
		foreach (var permisison in result)
		{
			var per = await _permissionRepository.GetAsync(x => x.PermissionId == permisison, null);
			var permissionViewModel = new PermissionViewModel
			{
				Description = per.Description,
				Name = per.Name,
				PermissionId = per.PermissionId,
			};
			newPermisisonViewModel.Add(permissionViewModel);
		}
		return newPermisisonViewModel;
	}
    public async Task<IEnumerable<PermissionViewModel>> GetPermissionAuthorizeByUserId(Guid projectId, Guid userId)
	{
        var newPermisisonViewModel = new List<PermissionViewModel>();
		var projectMember = await _projectMemberRepository.GetQuery().Include(x => x.Project)
			.FirstOrDefaultAsync(pr => pr.ProjectId == projectId && pr.UserId == userId);
		if(projectMember == null) return null;
		var permissions = await _permissionSchemaRepository.GetPermissionBySchewmaAndRoleId(projectMember.Project.SchemasId, projectMember.RoleId);
        HashSet<Guid> result = new HashSet<Guid>();
        foreach (var permission in permissions)
        {
            result.Add(permission.PermissionId);
        }
        foreach (var permisison in result)
        {
            var per = await _permissionRepository.GetAsync(x => x.PermissionId == permisison, null);
            var permissionViewModel = new PermissionViewModel
            {
                Description = per.Description,
                Name = per.Name,
                PermissionId = per.PermissionId,
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
		var activeProject = projects.Where(x => x.StatusId == Guid.Parse("53F76F08-FF3C-43EB-9FF4-C9E028E513D5")).Count();
		var inactiveProject = projects.Where(x => x.StatusId == Guid.Parse("DB6CBA9F-6B55-4E18-BBC1-624AFDCD92C7")).Count();
		var doneProject = projects.Where(x => x.StatusId == Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C")).Count();
		var deleteProject = totalProject - activeProject - inactiveProject- doneProject;
		var activeProjectPercent = (int)Math.Round((double)(100 * activeProject) / totalProject);
		var inactiveProjectPercent = (int)Math.Round((double)(100 * inactiveProject) / totalProject);
		var doneProjectPercent = (int)Math.Round((double)(100 * doneProject) / totalProject);
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
			ProjectDone= doneProject,
			ProjectDonePercent= doneProjectPercent,
		};
	}

	public async Task<List<GetProjectTasksResponse>> GetProjectsTasks(Guid projectId)
	{
		var results = new List<GetProjectTasksResponse>();
		var iterations = await _interationRepository.GetAllWithOdata(x => x.BoardId == projectId, null);
		if (iterations == null) return null;
		foreach (var interation in iterations)
		{
			var tasks = await _ticketRepository.GetAllWithOdata(x => x.InterationId == interation.InterationId, x => x.Status);
			foreach (var task in tasks)
			{
                if (task.IsDelete == true)
                {
					continue;
                }
                var assignTo = await _projectMemberRepository.GetAsync(x => x.MemberId == task.AssignTo, x => x.Users);
				assignTo.Users.Status = await _statusRepository.GetAsync(x => x.StatusId == assignTo.Users.StatusId, null);

				var createBy = await _userRepository.GetAsync(x => x.UserId == task.CreateBy, null);

				var taskType = await _ticketTypeRepository.GetAsync(x => x.TypeId == task.TypeId, null);
				var priority = await _priorityRepository.GetAsync(x => x.LevelId == task.PriorityId, null);

				var newTask = new GetProjectTasksResponse();
				newTask.TaskId = task.TaskId;
				newTask.Title = task.Title;
				newTask.Description = task.Description;
                newTask.StartDate = task.StartDate == null ? null : task.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                newTask.DueDate = task.DueDate == null ? null : task.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                newTask.CreateTime = task.CreateTime == null ? null : task.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                newTask.DeleteAt = task.DeleteAt == null ? null : task.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                newTask.AssignTo = _mapper.Map<UserResponse>(assignTo.Users);
				newTask.CreateBy = _mapper.Map<UserResponse>(createBy);
				newTask.TaskType = taskType.Title;
				newTask.PrevId = task.PrevId;
				newTask.StatusId = task.StatusId;
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

	public async Task<BaseResponse> RemoveProjectMember(Guid memberId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectMemberRepository.GetAsync(x => x.MemberId == memberId, x=>x.Users);
			if(project.StatusId == Guid.Parse("2D79988F-49C8-4BF4-B5AB-623559B30746"))
			{
				var invitation = await _invitationRepository.GetAsync(x => x.StatusId == Guid.Parse("2D79988F-49C8-4BF4-B5AB-623559B30746") && x.InviteTo.Equals(project.Users.Email), null);

				invitation.StatusId = Guid.Parse("2707D89B-6040-474C-ABD0-1F2CBC8DAEAB");

				await _invitationRepository.UpdateAsync(invitation);
				await _invitationRepository.SaveChanges();
			}
			project.StatusId = Guid.Parse("A29BF1E9-2DE2-4E5F-A6DA-32D88FCCD274");

			await _projectMemberRepository.UpdateAsync(project);
			await _projectMemberRepository.SaveChanges();

			transaction.Commit();

			return new BaseResponse
			{
				IsSucceed = true,
				Message = "Delete successfully"
			};
		}
		catch (Exception ex)
		{
			transaction.RollBack();
			return new BaseResponse
			{
				IsSucceed = false,
				Message = "Delete fail",
			};
		}
	}

	public async Task<GetProjectReportRequest> GetProjectReport(Guid projectId)
	{
		return await _projectRepository.GetProjectReport(projectId);
	}

	public async Task<BaseResponse> ExitProject(Guid userId, Guid projectId)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var member = await _projectMemberRepository.GetAsync(x => x.UserId == userId && x.ProjectId == projectId, null);
			var project = await _projectMemberRepository.GetAsync(x => x.MemberId == member.MemberId, null);

			project.StatusId = Guid.Parse("A29BF1E9-2DE2-4E5F-A6DA-32D88FCCD274");

			await _projectMemberRepository.UpdateAsync(project);
			await _projectMemberRepository.SaveChanges();

			transaction.Commit();

			return new BaseResponse
			{
				IsSucceed = true,
				Message = "Exit successfully"
			};
		}
		catch (Exception ex)
		{
			transaction.RollBack();
			return new BaseResponse
			{
				IsSucceed = false,
				Message = "Exit fail",
			};
		}
	}

	public async Task<ChangeProjectStatusRespone> ChangeProjectStatus(Guid statusId,ChangeProjectStatusRequest changeProjectStatusRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var project = await _projectRepository.GetAsync(x => x.ProjectId == changeProjectStatusRequest.ProjectId, x => x.Status)!;

			project.StatusId = statusId ;

			var update = await _projectRepository.UpdateAsync(project);
			await _projectRepository.SaveChanges();
			transaction.Commit();
			return new ChangeProjectStatusRespone
			{
				ProjectId = update.ProjectId,
				ProjectName = update.ProjectName,
				StatusId = update.StatusId,
				StatusResponse = new BaseResponse
				{
					IsSucceed = true,
					Message = "Change project's status successfully"
				}
			};
		}
		catch (Exception)
		{
			transaction.RollBack();
			return new ChangeProjectStatusRespone
			{
				StatusResponse = new BaseResponse
				{
					IsSucceed = true,
					Message = "Update Project fail"
				}
			};
		}
	}

    public async Task<BaseResponse> UpdateProjectSchema(Guid projectId, UpdatePermissionSchemaRequest changePermissionSchemaRequest)
    {
        using var transaction = _projectRepository.DatabaseTransaction();
        try
        {
            var schemaPermission = await _permissionSchemaRepository.GetAllWithOdata(x => x.SchemaId == changePermissionSchemaRequest.SchemaId, null);
            var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, x => x.ProjectMembers)!;
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

			Guid shemaID = Guid.Empty;

            if (project.SchemasId != Guid.Parse("267F7D1D-0292-4F47-88A0-BD2E4F3B0990"))
			{
				shemaID = project.SchemasId;
			}

            project.SchemasId = newSchema.SchemaId;
            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChanges();

			if(shemaID != Guid.Empty)
			{
                await _schemaRepository.DeleteSchemaById(shemaID);
                await _schemaRepository.SaveChanges();
            }
            

            transaction.Commit();
			return new BaseResponse
			{
				IsSucceed = true,
				Message = "Change project's schema successfully"
			};
        }
        catch (Exception)
        {
            transaction.RollBack();
            return new BaseResponse
            {
                IsSucceed = false,
                Message = "Update Project fail"
            };
        }
    }
    public async Task<List<GetProjectCalendarResponse>> GetProjectCalender(Guid projectId)
    {
        return await _projectRepository.GetProjectCalender(projectId);
    }

	public async Task<InvitationResponse> CheckInvation(Guid invationId)
	{
		var result = await _invitationRepository.GetInvitation(invationId);
		return result;
	}

    public Task<BaseResponse> ChangeProjectSchema(Guid projectId)
    {
        throw new NotImplementedException();
    }

	public async Task<List<ProjectStatusViewModel>> GetAllProjectStatus(Guid projectId)
	{
		var doneProject = await _statusRepository.GetAsync(x => x.StatusId == Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C"), null);
		var doingProject = await _statusRepository.GetAsync(x => x.StatusId == Guid.Parse("53F76F08-FF3C-43EB-9FF4-C9E028E513D5"), null);
		var deletedProject = await _statusRepository.GetAsync(x => x.StatusId == Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31"), null);

		var projectStatusList = new List<ProjectStatusViewModel>();

		projectStatusList.Add(new ProjectStatusViewModel
		{
			StatusId = doneProject.StatusId,
			StatusName = doneProject.Title
		});

		projectStatusList.Add(new ProjectStatusViewModel
		{
			StatusId = doingProject.StatusId,
			StatusName = doingProject.Title
		});

		projectStatusList.Add(new ProjectStatusViewModel
		{
			StatusId = deletedProject.StatusId,
			StatusName = deletedProject.Title
		});

		return projectStatusList;
	}

	public async Task<int> GetTaskStatusDone(Guid projectId)
	{
		int taskDone = await _ticketRepository.GetTaskDone(projectId);
		int taskTotal = await _ticketRepository.GetTotalTask(projectId);
		return taskTotal - taskDone;
	}

	public async Task<bool> CheckExist(Guid projectId)
	{
		var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null);
		if (project == null)
			return false;
		return true;
	}

	public async Task<bool> CheckMemberStatus(Guid memberId)
	{
		var member = await _projectMemberRepository.GetAsync(x => x.MemberId == memberId, x=>x.Status);
		if (member.StatusId != Guid.Parse("BA888147-C90A-4578-8BA6-63BA1756FAC1")) return false;
		return true;
	}
}