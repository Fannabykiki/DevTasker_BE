using Capstone.Common.DTOs.Project;
using Capstone.Service.LoggerService;
using Capstone.Service.Project;
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
            var result = await _projectService.GetAllProjects(userId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}

