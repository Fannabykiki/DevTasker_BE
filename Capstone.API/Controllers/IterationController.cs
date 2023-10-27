using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
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
        public async Task<IActionResult> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid boardId)
        {
            var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, boardId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }

       /* [EnableQuery]
        [HttpGet("Iteration")]
        public async Task<ActionResult<IQueryable<GetAllIterationResponse>>> GetAllIteration()
        {
            var result = await _iterationService.GetAllIteration();

            return Ok(result);
        }*/
    }
}
