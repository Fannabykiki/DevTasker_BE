using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Iteration;
using Capstone.Service.IterationService;
using Capstone.Service.LoggerService;
using Capstone.Service.ProjectService;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
	[Route("api/Interation-management")]
    [ApiController]
    public class IterationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IIterationService _iterationService;
        private readonly IProjectService _projectService;

		public IterationController(ILoggerManager logger, IIterationService iterationService, IProjectService projectService)
		{
			_logger = logger;
			_iterationService = iterationService;
			_projectService = projectService;
		}

		[HttpPost("Interation")]
        public async Task<ActionResult<GetIntergrationResponse>> CreateIteration(CreateIterationRequest createIterationRequest)
        {   
            var interationList = await _projectService.GetInterationByProjectId(createIterationRequest.ProjectId);
            foreach (var interation in interationList)
            {
                if (createIterationRequest.InterationName.Equals(interation.InterationName))
                {
					return BadRequest("Interation's name is exist. Please try another interation name");
				}
			}
            var project = await _projectService.GetProjectByProjectId(createIterationRequest.ProjectId);
            if(createIterationRequest.StartDate <= project.StartDate)
            {
                return BadRequest("Cant create new interation with start date before project's start date. Please update and try again");
            }
			if (createIterationRequest.EndDate >= project.EndDate)
			{
				return BadRequest("Cant create new interation with end date after project's end date. Please update and try again");
			}
			var result = await _iterationService.CreateInteration(createIterationRequest);

            return Ok(result);
        }
        //1
        [HttpPut("Interation")]
        public async Task<ActionResult<BaseResponse>> UpdateIteration(UpdateIterationRequest updateIterationRequest)
        {
			var interation = await _iterationService.GetIterationsById(updateIterationRequest.InterationId);
			var interationList = await _projectService.GetInterationByProjectId(interation.BoardId);
			foreach (var inter in interationList)
			{
				if (updateIterationRequest.InterationName.Equals(inter.InterationName))
				{
					return BadRequest("Interation's name is exist. Please try another interation name");
				}
			}
			var project = await _projectService.GetProjectByProjectId(interation.BoardId);
			if (updateIterationRequest.StartDate <= project.StartDate)
			{
				return BadRequest("Cant create new interation with start date before project's start date. Please update and try again");
			}
			if (updateIterationRequest.EndDate >= project.EndDate)
			{
				return BadRequest("Cant create new interation with end date after project's end date. Please update and try again");
			}
			var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, updateIterationRequest.InterationId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }


        [HttpGet("Interation/{projectId}")]
        public async Task<ActionResult<IQueryable<GetInterrationByIdResonse>>> GetAllIteration(Guid projectId)
        {
            var result = await _iterationService.GetIterationTasksByProjectId(projectId);
            if(result == null)
            {
                return BadRequest("Project not have any task!");
            }
            return Ok(result);
        }

        [HttpGet("Interation/tasks/{iterationId}")]
        public async Task<ActionResult<GetInterrationByIdResonse>> GetIterationById(Guid iterationId)
        {
            var result = await _iterationService.GetIterationsById(iterationId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
