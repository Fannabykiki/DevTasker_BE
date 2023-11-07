using AutoMapper;
using AutoMapper.Execution;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Linq;
using System.Xml.Linq;

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

    public ProjectService(CapstoneContext context, IProjectRepository projectRepository, IRoleRepository roleRepository, IMapper mapper, ISchemaRepository permissionSchemaRepository, IProjectMemberRepository projectMemberRepository, IBoardRepository boardRepository, IPermissionRepository permissionRepository, IInterationRepository interationRepository, IPermissionSchemaRepository permissionScemaRepo, IStatusRepository statusRepository)
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
                StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE"),
                CreateBy = userId,
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

            var newInteration = new Interation
            {
                StartDate = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                StatusId = Guid.Parse("3FC7B979-BC37-4E06-B38A-B01245541867"),
                BoardId = newProjectRequest.Board.BoardId,
                EndDate = DateTime.Parse(DateTime.UtcNow.AddDays(14).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
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
                IsSucced = true
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            transaction.RollBack();
            return new CreateProjectRespone
            {
                IsSucced = false,
            };
        }
    }

    public async Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid userId)
    {
        var projects = await _projectRepository.GetAllWithOdata(x => x.StatusId == Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE"), x => x.ProjectMembers.Where(m => m.UserId == userId));
        return _mapper.Map<List<GetAllProjectViewModel>>(projects);
    }
    public async Task<IEnumerable<GetUserProjectAnalyzeResponse>> GetUserProjectAnalyze(Guid userId)
    {
        int totalTicket = 0;
        int ticketDone = 0;
        var listProjectAnalyze = new List<GetUserProjectAnalyzeResponse>();
        var projects = await _projectRepository.GetAllWithOdata(x => true, x => x.ProjectMembers.Where(m => m.UserId == userId));
        foreach(var project in projects)
        {
            var projectStatus = await _statusRepository.GetAsync(x => x.StatusId == project.StatusId,null);
            var projectAnalyze = new GetUserProjectAnalyzeResponse();
            projectAnalyze.ProjectId = project.ProjectId;
            projectAnalyze.ProjectName = project.ProjectName;
            projectAnalyze.ProjectStatus = projectStatus.Title;
            var iteration = await _interationRepository.GetAllWithOdata(x => x.BoardId == project.Board.BoardId, x =>x.Tickets);
            foreach(var i in iteration)
            {
                totalTicket += i.Tickets.Count();
                ticketDone += i.Tickets.Where(x => x.StatusId == Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C")).Count();
            }
            projectAnalyze.TotalTickets = ticketDone + "/" + totalTicket;
            projectAnalyze.Process = (int)Math.Round((double)(100 * ticketDone) / ticketDone);
            listProjectAnalyze.Add(projectAnalyze);
            totalTicket = 0;
            ticketDone= 0;
        }
        return listProjectAnalyze;
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
            await _roleRepository.SaveChanges();

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
            await _roleRepository.GetAsync(x => x.RoleId == updateMemberRoleRequest.RoleId, null)!;

            var member = await _projectMemberRepository.GetAsync(x => x.MemberId == memberId, null)!;
            if (member.RoleId.Equals("5B5C81E8-722D-4801-861C-6F10C07C769B") || member.IsOwner == true)
            {
                return false;
            }

            member.RoleId = updateMemberRoleRequest.RoleId;
            await _projectMemberRepository.UpdateAsync(member);
            await _projectMemberRepository.SaveChanges();

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
            await _projectRepository.SaveChanges();
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
            await _projectRepository.SaveChanges();
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
            project.StatusId = Guid.Parse("DB6CBA9F-6B55-4E18-BBC1-624AFDCD92C7");
            project.DeleteAt = DateTime.UtcNow;
            project.ExpireAt = DateTime.UtcNow.AddDays(30);
            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChanges();
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
            project.StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE");
            project.DeleteAt = null;
            project.ExpireAt = null;
            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChanges();
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
        var members = await _projectMemberRepository.GetAllWithOdata(m => m.ProjectId == projectId, p => p.Role)!;
        if (!members.Any()) return projectInfoRequests;
        {
            var projectInfoRequest = new ViewProjectInfoRequest
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StatusId = project.StatusId,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                CreateBy = project.CreateBy,
                CreateAt = project.CreateAt,
                PrivacyStatus = project.PrivacyStatus,
                ProjectMembers = project.ProjectMembers
                    .Select(m => new ViewMemberProject
                    {
                        MemberId = m.MemberId,
                        UserId = m.UserId,
                        RoleId = m.RoleId,
                        ProjectId = m.ProjectId,
                        IsOwner = m.IsOwner
                        ,
                        RoleName = m.Role.RoleName

                    })
                    .ToList()
            };
            projectInfoRequests.Add(projectInfoRequest);
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

    public async Task<IEnumerable<GetAllProjectViewModel>> GetProjectsAdmin()
    {
        var projects = await _projectRepository.GetAllWithOdata(x => true,null);
		return _mapper.Map<List<GetAllProjectViewModel>>(projects);
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

}