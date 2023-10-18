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
        [HttpGet("projects/{userId}")]
        public async Task<ActionResult<IQueryable<GetAllProjectViewModel>>> GetProjectByUserId(Guid userId)
        {
            var result = await _projectService.GetProjectByUserId(userId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }

		[EnableQuery]
		[HttpGet("projects/{projectId}")]
		public async Task<ActionResult<IQueryable<ViewMemberProject>>> GetMemberByProjectId(Guid projectctId)
		{
			var result = await _projectService.GetMemberByProjectId(projectctId);
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
	}
}

