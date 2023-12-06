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
	public class InterationController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IIterationService _iterationService;
		private readonly IProjectService _projectService;

		public InterationController(ILoggerManager logger, IIterationService iterationService, IProjectService projectService)
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
				if (createIterationRequest.InterationName.ToLower().Equals(interation.InterationName.ToLower()))
				{
					return BadRequest("Sprint's name is exist. Please try another Sprint's name");
				}
			}
			var project = await _projectService.GetProjectByProjectId(createIterationRequest.ProjectId);
			if (createIterationRequest.StartDate <= project.StartDate)
			{
				return BadRequest("Cant create new sprint with start date before project's start date. Please update and try again");
			}
			if (createIterationRequest.EndDate >= project.EndDate)
			{
				return BadRequest("Can't create new sprint with end date after project's end date. Please update and try again");
			}
			var result = await _iterationService.CreateInteration(createIterationRequest);
			if (result.Response.StatusCode == 400)
			{
				return BadRequest(result.Response.Message);
			}
			return Ok(result);
		}
		//1
		[HttpPut("Interation")]
		public async Task<ActionResult<BaseResponse>> UpdateIteration(UpdateIterationRequest updateIterationRequest)
		{
			var isExist = await _iterationService.CheckExist(updateIterationRequest.InterationId);
			if (!isExist)
			{
				return NotFound("Sprint not exist!!!");
			}
			var interation = await _iterationService.GetIterationsById(updateIterationRequest.InterationId);
			var project = await _projectService.GetProjectByProjectId(interation.BoardId);
			if (updateIterationRequest.StartDate.Date < project.StartDate.Date)
			{
				return BadRequest("Can't update sprint with start date before project's start date. Please update and try again");
			}
			if (updateIterationRequest.EndDate.Date > project.EndDate.Date)
			{
				return BadRequest("Can't update sprint with end date after project's end date. Please update and try again");
			}
			var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, updateIterationRequest.InterationId);
			if (result == null)
			{
				return StatusCode(500);
			}
			else if (result.StatusCode == 400)
			{
				return BadRequest(result.Message);
			}

			return Ok(result);
		}


		[HttpGet("Interation/{projectId}")]
		public async Task<ActionResult<IQueryable<GetInterrationByIdResonse>>> GetAllIteration(Guid projectId)
		{
			var result = await _iterationService.GetIterationTasksByProjectId(projectId);
			if (result == null)
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
