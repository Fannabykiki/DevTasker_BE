using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.Service.IterationService;
using Capstone.Service.LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Capstone.API.Controllers
{
    [Route("api/Iteration-management")]
    [ApiController]
    public class IterationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IIterationService _iterationService;

        public IterationController(ILoggerManager logger, IIterationService iterationService)
        {
            _logger = logger;
            _iterationService = iterationService;
        }

        [HttpPost("Iteration")]
        public async Task<IActionResult> CreateIteration(CreateIterationRequest createIterationRequest, Guid boardId)
        {
            var result = await _iterationService.CreateIteration(createIterationRequest, boardId);

            return Ok(result);
        }

        [HttpPut("Iteration/{iterationId}")]
        public async Task<IActionResult> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId)
        {
            var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, iterationId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }


        [HttpGet("Iteration/{boardId}")]
        public async Task<ActionResult<IQueryable<GetInterrationByBoardIdResonse>>> GetAllIteration(Guid boardId)
        {
            var result = await _iterationService.GetIterationsByBoardId(boardId);

            return Ok(result);
        }

        [HttpGet("Iteration")]
        public async Task<ActionResult<IEnumerable<GetInterrationByIdResonse>>> GetIterationById(Guid iterationId)
        {
            var result = await _iterationService.GetIterationsById(iterationId);

            if (result == null || !result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
