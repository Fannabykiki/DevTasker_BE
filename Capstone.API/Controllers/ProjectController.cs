using Capstone.API.Extentions.AuthorizeMiddleware;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.Service.LoggerService;
using Capstone.Service.ProjectService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Security.Claims;

namespace Capstone.API.Controllers
{
    [Route("api/project-management")]
	[ApiController]
	public class ProjectController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IProjectService _projectService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ProjectController(ILoggerManager logger, IProjectService projectService, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_projectService = projectService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpPost("projects")]
		public async Task<IActionResult> CreateProject(CreateProjectRequest createProjectRequest)
		{
			var result = await _projectService.CreateProject(createProjectRequest);

			return Ok(result);
		}

		[EnableQuery]
		[HttpGet("projects/user/{memberId:Guid}")]
		public async Task<ActionResult<IQueryable<GetAllProjectViewModel>>> GetProjectByUserId(Guid memberId)
		{
			var result = await _projectService.GetProjectByUserId(memberId);
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
			var result = await _projectService.GetPermissionByUserId(projectId,userId);
			if (result == null)
			{
				return StatusCode(500);
			}

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

		[MiddlewareFilter(typeof(AuthorizationMiddleware))]
		[EnableQuery]
		[HttpGet("projects/info/{projectId:Guid}")]
		public async Task<ActionResult<List<ViewProjectInfoRequest>>> GetFullInfoProjectByProjectId(Guid projectId)
		{
			var claims = new[] {
			new Claim("projectId", projectId.ToString())
		};
			var identity = new ClaimsIdentity(claims, "DefaultAuthType");
			var principal = _httpContextAccessor.HttpContext.User;
			principal.AddIdentity(identity);
			_httpContextAccessor.HttpContext.User = principal;

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
		if (project.ExpireAt >= DateTime.UtcNow)
		{
			return BadRequest("Can not restore Project. Over 30 days");
		}
		var result = await _projectService.RestoreProject(projectId);

		return Ok(result);
	}
}
}

