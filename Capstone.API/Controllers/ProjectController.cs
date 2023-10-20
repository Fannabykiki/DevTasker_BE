using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.ProjectService;
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

        public ProjectController(ILoggerManager logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
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
		public async Task<IActionResult> UpdateMemberRole(Guid memberId,UpdateMemberRoleRequest updateMemberRoleRequest)
		{	
			var result = await _projectService.UpdateMemberRole(memberId,updateMemberRoleRequest);
			if (result == null)
			{
				return StatusCode(500);
			}

			return Ok(result);
		}	
		
		[HttpPut("projects/info/{projectId}")]
		public async Task<IActionResult> UpdateProjectInfo(Guid projectId, UpdateProjectNameInfo updateProjectNameInfo)
		{	
			var result = await _projectService.UpdateProjectInfo(projectId,updateProjectNameInfo);

			return Ok(result);
		}
		
		[HttpPut("projects/privacy/{projectId}")]
		public async Task<IActionResult> UpdateProjectPrivacy(Guid projectId, UpdateProjectPrivacyRequest updateProjectPrivacyRequest)
		{	
			var result = await _projectService.UpdateProjectPrivacy(projectId,updateProjectPrivacyRequest);

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

