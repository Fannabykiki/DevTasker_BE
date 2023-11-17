using Capstone.API.Extentions;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Invitaion;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.Service.LoggerService;
using Capstone.Service.ProjectMemberService;
using Capstone.Service.ProjectService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

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

		public ProjectController(ILoggerManager logger, IProjectService projectService, IHttpContextAccessor httpContextAccessor, IProjectMemberService projectMemberService, IUserService userService)
		{
			_logger = logger;
			_projectService = projectService;
			_httpContextAccessor = httpContextAccessor;
			_projectMemberService = projectMemberService;
			_userService = userService;
		}

		[HttpPost("projects")]
		public async Task<ActionResult<CreateProjectRespone>> CreateProject(CreateProjectRequest createProjectRequest)
		{
			var userId = this.GetCurrentLoginUserId();
			var result = await _projectService.CreateProject(createProjectRequest, userId);

			return Ok(result);
		}

		[HttpPost("projects/remove-member")]
		public async Task<ActionResult<BaseResponse>> RemoveProjectMember(Guid memberId)
		{
			var result = await _projectService.RemoveProjectMember(memberId);

			return Ok(result);
		}

		[HttpPost("projects/exit-project")]
		public async Task<ActionResult<BaseResponse>> ExitProject(Guid projectId)
		{
			var userId = this.GetCurrentLoginUserId();
			var result = await _projectService.ExitProject(userId,projectId);

			return Ok(result);
		}

		[HttpPost("projects/invitation")]
		public async Task<IActionResult> InviteMember(InviteUserRequest inviteUserRequest)
		{
			var userId = this.GetCurrentLoginUserId();
			foreach (var email in inviteUserRequest.Email)
			{
				var user = await _userService.GetUserByEmailAsync(email);
				if (user == null)
				{
					return BadRequest(email + "not exist in system");
				}
			}
			var projectMember = await _projectMemberService.AddNewProjectMember(inviteUserRequest);
			await _projectService.SendMailInviteUser(inviteUserRequest, userId);

			return Ok(projectMember);
		}

		[HttpPost("projects/close-project")]
		public async Task<ActionResult<ChangeProjectStatusRespone>> CloseProject(ChangeProjectStatusRequest changeProjectStatusRequest)
		{

			var project = await _projectService.ChangeProjectStatus(changeProjectStatusRequest);
			return Ok(project);
		}

		[HttpGet("projects/check-invitation")]
		public async Task<ActionResult<InvitationResponse>> CheckInvation(Guid invationId)
		{
			var project = await _projectService.CheckInvation(invationId);
			return Ok(project);
		}

		[HttpPost("projects/decline-invitation")]
		public async Task<IActionResult> InviteMemberDeclination(AcceptInviteRequest acceptInviteRequest)
		{
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
			var projectMember = await _projectMemberService.AcceptInvitation(user.UserId, acceptInviteRequest);

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
		[HttpGet("projects/permission")]
		public async Task<ActionResult<IQueryable<PermissionViewModel>>> GetPermisisionByUseriId(Guid projectId, Guid userId)
		{
			var result = await _projectService.GetPermissionByUserId(projectId, userId);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/report/{projectId}")]
		public async Task<ActionResult<IQueryable<PermissionViewModel>>> GetProjectReport(Guid projectId)
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


		[HttpPost("roles")]
		public async Task<IActionResult> CreateRole(CreateRoleRequest createRoleRequest)
		{
			var result = await _projectService.CreateProjectRole(createRoleRequest);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[HttpPut("roles/{memberId}")]
		public async Task<IActionResult> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest)
		{
			if (updateMemberRoleRequest.RoleId.Equals("5B5C81E8-722D-4801-861C-6F10C07C769B") || updateMemberRoleRequest.RoleId.Equals("7ACED6BC-0B25-4184-8062-A29ED7D4E430"))
				return BadRequest("You can not change to this role !");
			var result = await _projectService.UpdateMemberRole(memberId, updateMemberRoleRequest);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}

		[HttpPut("projects/info/{projectId}")]
		public async Task<IActionResult> UpdateProjectInfo(Guid projectId, UpdateProjectNameInfo updateProjectNameInfo)
		{
			var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);

			return Ok(result);
		}

		[HttpPut("projects/privacy/{projectId}")]
		public async Task<IActionResult> UpdateProjectPrivacy(Guid projectId, UpdateProjectPrivacyRequest updateProjectPrivacyRequest)
		{
			var result = await _projectService.UpdateProjectPrivacy(projectId, updateProjectPrivacyRequest);

			return Ok(result);
		}

		[HttpDelete("projects/{projectId}")]
		public async Task<IActionResult> DeleteProject(Guid projectId)
		{
			var project = await _projectService.GetProjectByProjectId(projectId);
			var pro = await _projectService.GetProjectsTasks(projectId);
			
			if (project.DeleteAt is not null)
			{
				return BadRequest("Project is already deleted");
			}

			var result = await _projectService.DeleteProject(projectId);

			return Ok(result);
		}

		[HttpPut("project/restoration/{projectId}")]
		public async Task<IActionResult> RestoreProjectStatus(Guid projectId)
		{
			var project = await _projectService.GetProjectByProjectId(projectId);
			if (project.DeleteAt == null)
			{
				return BadRequest("Projects is still active. Cant restore it!!!");
			}
			if (project.ExpireAt >= DateTime.Now)
			{
				var result = await _projectService.RestoreProject(projectId);
				return Ok(result);
			}
			else
			{
				return BadRequest("Cant restore this Project.Over 30 days from delete day");
			}
		}
	}
}

