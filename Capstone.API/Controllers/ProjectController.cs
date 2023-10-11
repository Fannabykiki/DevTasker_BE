using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Common.DTOs.Project;
using Capstone.Service.LoggerService;
using Capstone.Service.Project;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            // if (createProjectRequest.EndDate >= createProjectRequest.StartDate)
            // {
            //     return BadRequest("EndDate is not greater than StartDate");
            // }
            var result = await _projectService.CreateProject(createProjectRequest);

            return Ok(result);
        }
        
        
    }
}
