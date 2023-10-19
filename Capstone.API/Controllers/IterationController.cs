using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.Service.IterationService;
using Capstone.Service.LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreateIteration(CreateIterationRequest createIterationRequest, Guid projectId)
        {
            var result = await _iterationService.CreateIteration(createIterationRequest, projectId);

            return Ok(result);
        }

        [HttpPut("Iteration/{UpdateIterationId}")]
        public async Task<IActionResult> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId)
        {
            var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, iterationId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}
