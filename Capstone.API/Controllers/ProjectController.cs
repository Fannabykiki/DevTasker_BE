using AutoMapper.Execution;
using Capstone.API.Extentions;
using Capstone.API.Extentions.RolePermissionAuthorize;
using Capstone.Common.Constants;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Invitaion;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.ProjectMemberService;
using Capstone.Service.ProjectService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.CodeAnalysis;

namespace Capstone.API.Controllers
{
	[Route("api/project-management")]
	[ApiController]
	public class ProjectController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IProjectService _projectService;
		private readonly IProjectMemberService _projectMemberService;
		private readonly IUserService _userService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IAuthorizationService _authorizationService;

		public ProjectController(ILoggerManager logger, 
			IProjectService projectService, 
			IHttpContextAccessor httpContextAccessor, 
			IProjectMemberService projectMemberService, 
			IUserService userService,
			IAuthorizationService authorizationService)
		{
			_logger = logger;
			_projectService = projectService;
			_httpContextAccessor = httpContextAccessor;
			_projectMemberService = projectMemberService;
			_userService = userService;
			_authorizationService = authorizationService;
		}

		[HttpPost("projects")]
		public async Task<ActionResult<CreateProjectRespone>> CreateProject(CreateProjectRequest createProjectRequest)
		{
			var userId = this.GetCurrentLoginUserId();
			var result = await _projectService.CreateProject(createProjectRequest, userId);

			return Ok(result);
		}

		// E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPost("projects/remove-member")]
		public async Task<ActionResult<BaseResponse>> RemoveProjectMember(Guid memberId)
		{
			var projectId = await _projectMemberService.GetProjectIdFromMember(memberId);
			var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
				new RolePermissionResource
				{
					ListProjectId = new List<Guid?> { projectId },
					ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects}
				}, AuthorizationRequirementNameConstant.RolePermission);
			if (!authorizationResult.Succeeded)
			{
				return Unauthorized(ErrorMessage.InvalidPermission);
			}

			var member = await _projectMemberService.GetMemberByMemberId(memberId);
			if (member.IsOwner)
			{
				return BadRequest("Can't remove Project Owner");
			}
			else if (member.UserId == Guid.Parse("AFA06CDD-7713-4B81-9163-C45556E4FA4C"))
			{
				return BadRequest("Can't remove System Admin");
			}
			var result = await _projectService.RemoveProjectMember(memberId);

			return Ok(result);
		}

		[HttpPost("projects/exit-project")]
		public async Task<ActionResult<BaseResponse>> ExitProject(Guid projectId)
		{
			var userId = this.GetCurrentLoginUserId();
			var result = await _projectService.ExitProject(userId, projectId);

			return Ok(result);
		}

		//  E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPost("projects/invitation")]
		public async Task<IActionResult> InviteMember(InviteUserRequest inviteUserRequest)
		{
            //Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { inviteUserRequest.ProjectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
			if (!authorizationResult.Succeeded)
			{
				return Unauthorized(ErrorMessage.InvalidPermission);
			};

            var projectPrivacy = await _projectService.GetProjectByProjectId(inviteUserRequest.ProjectId);
			if(projectPrivacy.PrivacyStatus == false)
			{
				return BadRequest("Your project is private status. Can't invite any member");
			}
			var userId = this.GetCurrentLoginUserId();
			if (userId == Guid.Empty)
			{
				return BadRequest("You need to login first");
			}
			foreach (var email in inviteUserRequest.Email)
			{
				var user = await _userService.GetUserByEmailAsync(email);
				if (user == null)
				{
					return BadRequest(email + " not exist in system");
				}
				var isInTeam = await _projectMemberService.CheckMemberStatus(email, inviteUserRequest.ProjectId, Guid.Parse("BA888147-C90A-4578-8BA6-63BA1756FAC1"));
				var isPending = await _projectMemberService.CheckMemberStatus(email, inviteUserRequest.ProjectId, Guid.Parse("A29BF1E9-2DE2-4E5F-A6DA-32D88FCCD274"));
				var isSendMail = await _projectMemberService.CheckMemberStatus(email, inviteUserRequest.ProjectId, Guid.Parse("2D79988F-49C8-4BF4-B5AB-623559B30746"));

				if (isInTeam == false)
				{
					return BadRequest($"Email {email} is already existed in project. Can't invite anymore!!!");
				}
				else if (isPending == false)
				{
					await _projectService.SendMailInviteUser(inviteUserRequest, userId);
					return Ok($"Email {email} is already left project. Please check mail and confirm invitation to join project again");
				}
				else if (isSendMail == false)
				{
					return BadRequest($"Invitation is already sent to {email}. Please check mail and confirm invitation");
				}
				if (user.StatusId == Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB"))
				{
					return BadRequest("Can't invite inactive user !!!");
				}

			}

			var projectMember = await _projectMemberService.AddNewProjectMember(inviteUserRequest);
			await _projectService.SendMailInviteUser(inviteUserRequest, userId);

			return Ok(projectMember);
		}

		//  E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		//  31085E0A-EEDC-495D-BD68-94A60A661B05 - Browse Projects
		[HttpPost("projects/close-project")]
		public async Task<ActionResult<ChangeProjectStatusRespone>> CloseProject(ChangeProjectStatusRequest changeProjectStatusRequest)
		{
			//Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { changeProjectStatusRequest.ProjectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects, "Browse Projects" }
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }

            var pro = await _projectService.GetTaskStatusDone(changeProjectStatusRequest.ProjectId);
			if (pro != 0)
			{
				return BadRequest("Task of project hasn't done yet. Please set all task with status done before close project");
			}
			var project = await _projectService.ChangeProjectStatus(changeProjectStatusRequest);
			return Ok(project);
		}

		[HttpGet("projects/check-invitation")]
		public async Task<ActionResult<InvitationResponse>> CheckInvation(Guid invationId)
		{
			var project = await _projectService.CheckInvation(invationId);
			return Ok(project);
		}

		//  E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPost("projects/decline-invitation")]
		public async Task<IActionResult> InviteMemberDeclination(AcceptInviteRequest acceptInviteRequest)
		{
            //Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { acceptInviteRequest.ProjectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects}
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }

            var user = await _userService.GetUserByEmailAsync(acceptInviteRequest.Email);
			var uId = this.GetCurrentLoginUserId();
			if (user == null)
			{
				return NotFound("ProjectMember account dont exist in system");
			}
			if (uId == Guid.Empty)
			{
				return BadRequest("You need to login first");
			}
			if (user.UserId != uId)
			{
				return BadRequest("Account dont match with invitation");
			}
			var projectMember = await _projectMemberService.DeclineInvitation(user.UserId, acceptInviteRequest);

			return Ok(projectMember);
		}

		[HttpPost("projects/accept-invitation")]
		public async Task<IActionResult> InviteMemberAcception(AcceptInviteRequest acceptInviteRequest)
		{
			var user = await _userService.GetUserByEmailAsync(acceptInviteRequest.Email);
			var uId = this.GetCurrentLoginUserId();
			if (user == null)
			{
				return NotFound("User account dont exist in system");
			}
			if (uId == Guid.Empty)
			{
				return BadRequest("You need to login first");
			}
			if (user.UserId != uId)
			{
				return BadRequest("Account dont match with invitation");
			}
			var projectMember = await _projectMemberService.AcceptInvitation(user.UserId, acceptInviteRequest);

			return Ok(projectMember);
		}

		[EnableQuery]
		[HttpGet("projects")]
		public async Task<ActionResult<IQueryable<GetAllProjectViewModel>>> GetProjectByUserId()
		{
			var userId = this.GetCurrentLoginUserId();
			if (userId == Guid.Empty)
			{
				return Unauthorized("You need to login first");
			}
			var result = await _projectService.GetProjectByUserId(userId);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("admin/projects")]
		public async Task<ActionResult<List<GetAllProjectResponse>>> GetProjectsAdmin()
		{
			var result = await _projectService.GetProjectsAdmin();
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}


		[EnableQuery]
		[HttpGet("projects/analyzation/{userId:Guid}")]
		public async Task<ActionResult<IQueryable<GetUserProjectAnalyzeResponse>>> GetUserProjectAnalyze(Guid userId)
		{
			var UserId = this.GetCurrentLoginUserId();
			if (userId == Guid.Empty)
			{
				return Unauthorized("You dont have permission to access this page");
			}
			var result = await _projectService.GetUserProjectAnalyze(userId);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/tasks/{projectId:Guid}")]
		public async Task<ActionResult<List<GetProjectTasksResponse>>> GetProjectsTasks(Guid projectId)
		{
			var result = await _projectService.GetProjectsTasks(projectId);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}
		

		[HttpGet("projects/status")]
		public async Task<ActionResult<List<ProjectStatusViewModel>>> GetProjectStatus(Guid projectId)
		{
			var result = await _projectService.GetAllProjectStatus(projectId);

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("admin/projects/analyzation")]
		public async Task<ActionResult<IQueryable<GetAllProjectViewModel>>> GetProjectAnalyze()
		{
			var result = await _projectService.ProjectAnalyzeAdmin();
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/member-permission")]
		public async Task<ActionResult<IQueryable<PermissionViewModel>>> GetPermisisionByUseriId(GetPermissionByProjectRequest request)
		{
            var userId = this.GetCurrentLoginUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("You need login first");
            }
            var result = await _projectService.GetPermissionByUserId(request.ProjectId, userId);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/report/{projectId}")]
		public async Task<ActionResult<GetProjectReportRequest>> GetProjectReport(Guid projectId)
		{
			var result = await _projectService.GetProjectReport(projectId);
			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/{projectId:Guid}")]
		public async Task<ActionResult<IQueryable<ViewMemberProject>>> GetMemberByProjectId(Guid projectId)
		{
			var result = await _projectService.GetMemberByProjectId(projectId);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/interation/{projectId:Guid}")]
		public async Task<ActionResult<IQueryable<InterationViewModel>>> GetInterationByProjectId(Guid projectId)
		{
			var result = await _projectService.GetInterationByProjectId(projectId);
			if (result == null)
			{
				return StatusCode(500);
			}
			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/info/{projectId:Guid}")]
		public async Task<ActionResult<ViewProjectInfoRequest>> GetFullInfoProjectByProjectId(Guid projectId)
		{

			var result = await _projectService.GetInfoProjectByProjectId(projectId);

			return Ok(result);
		}

		// E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPost("roles")]
		public async Task<IActionResult> CreateRole(CreateRoleRequest createRoleRequest)
		{
			var projects = await _projectService.GetProjectByUserId(this.GetCurrentLoginUserId());

            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = projects.Select(x=> x.ProjectId as Guid?).ToList(),
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects}
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }
            var result = await _projectService.CreateProjectRole(createRoleRequest);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		//4  E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		//   User IsAdmin == true
        [HttpPut("roles")]
        public async Task<IActionResult> UpdateMemberRole( UpdateMemberRoleRequest updateMemberRoleRequest)
        {
			//Authorize
			if (!this.HttpContext.User.GetIsAdmin()){
                var projects = await _projectService.GetProjectByUserId(this.GetCurrentLoginUserId());
                var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = projects.Select(x => x.ProjectId as Guid?).ToList(),
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
                if (!authorizationResult.Succeeded)
                {
                    return Unauthorized(ErrorMessage.InvalidPermission);
                }
            }
			var member = await _projectMemberService.CheckExist(updateMemberRoleRequest.MemberId);

			if (!member)
			{
				return NotFound("Member not exist");
			}
			if (updateMemberRoleRequest.RoleId.Equals("5B5C81E8-722D-4801-861C-6F10C07C769B") || updateMemberRoleRequest.RoleId.Equals("7ACED6BC-0B25-4184-8062-A29ED7D4E430"))
				return BadRequest("You can not change to this role !");
			var result = await _projectService.UpdateMemberRole(updateMemberRoleRequest.MemberId, updateMemberRoleRequest);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[HttpPut("projects/info")]
		public async Task<IActionResult> UpdateProjectInfo(UpdateProjectNameInfo updateProjectNameInfo)
		{
			var project = await _projectService.CheckExist(updateProjectNameInfo.ProjectId);
			if (!project)
			{
				return NotFound("Project not exist");
			}



			var result = await _projectService.UpdateProjectInfo(updateProjectNameInfo.ProjectId, updateProjectNameInfo);

			return Ok(result);
		}

		//2 E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPut("projects/privacy")]
		public async Task<IActionResult> UpdateProjectPrivacy(UpdateProjectPrivacyRequest updateProjectPrivacyRequest)
		{
			var project = await _projectService.CheckExist(updateProjectPrivacyRequest.ProjectId);
			if (!project)
			{
				return NotFound("Project not exist");
			}

			//Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?>() { updateProjectPrivacyRequest.ProjectId},
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
			if (!authorizationResult.Succeeded)
			{
				return Unauthorized(ErrorMessage.InvalidPermission);
			}

            var result = await _projectService.UpdateProjectPrivacy(updateProjectPrivacyRequest.ProjectId, updateProjectPrivacyRequest);

			return Ok(result);
		}

		//1  E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPut("projects/delete")]
		public async Task<IActionResult> DeleteProject(DeleteProjectRequest deleteProjectRequest)
		{
			var pro = await _projectService.CheckExist(deleteProjectRequest.ProjectId);
			if (!pro)
			{
				return NotFound("Project not exist");
			}
			//Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { deleteProjectRequest.ProjectId},
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
			if (!authorizationResult.Succeeded)
			{
				return Unauthorized(ErrorMessage.InvalidPermission);


			}
            var project = await _projectService.GetProjectByProjectId(deleteProjectRequest.ProjectId);
		
			if (project.IsDelete == true)
			{
				return BadRequest("Project is already deleted");
			}

			var result = await _projectService.DeleteProject(deleteProjectRequest.ProjectId);

			return Ok(result);
		}

		//5  E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPut("project/restoration")]
		public async Task<IActionResult> RestoreProjectStatus(DeleteProjectRequest deleteProjectRequest)
		{
            //Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { deleteProjectRequest.ProjectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);


            }

            var project = await _projectService.GetProjectByProjectId(deleteProjectRequest.ProjectId);
			if (project.DeleteAt == null)
			{
				return BadRequest("Projects is still active. Cant restore it!!!");
			}
			if (project.ExpireAt >= DateTime.Now)
			{
				var result = await _projectService.RestoreProject(deleteProjectRequest.ProjectId);
				return Ok(result);
			}
			else
			{
				return BadRequest("Cant restore this Project.Over 30 days from delete day");
			}
		}

		// E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPut("project/change-schema/{projectId}")]
		public async Task<IActionResult> ChangeProjectSchema(Guid projectId, UpdatePermissionSchemaRequest request)
		{
            //Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { projectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);


            }
            var result = await _projectService.UpdateProjectSchema(projectId,request);
            if (result == null)
            {
                return StatusCode(500);
            }
            return Ok(result);
        }
	}
}

