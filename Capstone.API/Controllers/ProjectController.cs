using AutoMapper.Execution;
using Capstone.API.Extentions;
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
				var isInTeam = await _projectMemberService.CheckMemberExist(email, inviteUserRequest.ProjectId);
				if(isInTeam == false)
				{
					return BadRequest($"Email {email} is already existed in project. Can't invite anymore!!!");
				}
				var isPending = await _projectMemberService.CheckMemberStatus(email, inviteUserRequest.ProjectId);
				if(isPending == false)
				{
					await _projectService.SendMailInviteUser(inviteUserRequest, userId);
					return Ok($"Email {email} is already left project. Please check mail and confirm invitation to join project again");
				}
				var isSendMail = await _projectMemberService.CheckSendMail(email, inviteUserRequest.ProjectId);
				if (isSendMail == false)
				{
					return BadRequest($"Invitation is already sent to {email}. Please check mail and confirm invitation");
				}
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
        
        [HttpPost("projects/change-schema")]
        public async Task<IActionResult> ChangePermissionSchema(UpdatePermissionSchemaRequest request)
		{
            var result = await _projectService.UpdateProjectSchema(request);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }

		//4
        [HttpPut("roles")]
        public async Task<IActionResult> UpdateMemberRole( UpdateMemberRoleRequest updateMemberRoleRequest)
        {
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

		//3
		[HttpPut("projects/info")]
		public async Task<IActionResult> UpdateProjectInfo( UpdateProjectNameInfo updateProjectNameInfo)
		{
			var project = await _projectService.CheckExist(updateProjectNameInfo.ProjectId);
			if (!project)
			{
				return NotFound("Project not exist");
			}
			var result = await _projectService.UpdateProjectInfo(updateProjectNameInfo.ProjectId, updateProjectNameInfo);

			return Ok(result);
		}

		//2
		[HttpPut("projects/privacy")]
		public async Task<IActionResult> UpdateProjectPrivacy( UpdateProjectPrivacyRequest updateProjectPrivacyRequest)
		{
			var project = await _projectService.CheckExist(updateProjectPrivacyRequest.ProjectId);
			if (!project)
			{
				return NotFound("Project not exist");
			}
			var result = await _projectService.UpdateProjectPrivacy(updateProjectPrivacyRequest.ProjectId, updateProjectPrivacyRequest);

			return Ok(result);
		}

		//1
		[HttpPut("projects/delete")]
		public async Task<IActionResult> DeleteProject(DeleteProjectRequest deleteProjectRequest)
		{
			var pro = await _projectService.CheckExist(deleteProjectRequest.ProjectId);
			if (!pro)
			{
				return NotFound("Project not exist");
			}
			var project = await _projectService.GetProjectByProjectId(deleteProjectRequest.ProjectId);
		
			if (project.IsDelete == true)
			{
				return BadRequest("Project is already deleted");
			}

			var result = await _projectService.DeleteProject(deleteProjectRequest.ProjectId);

			return Ok(result);
		}

		//5
		[HttpPut("project/restoration")]
		public async Task<IActionResult> RestoreProjectStatus(DeleteProjectRequest deleteProjectRequest)
		{
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
		
		[HttpPut("project/change-schema/{projectId}")]
		public async Task<IActionResult> ChangeProjectSchema(Guid projectId)
		{
            return Ok();
        }
	}
}

