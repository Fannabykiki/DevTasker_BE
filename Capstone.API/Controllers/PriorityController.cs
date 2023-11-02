using Capstone.DataAccess.Entities;
using Capstone.Service.Priority;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Capstone.API.Controllers
{
    [Route("api/priority-management")]
    [ApiController]
    public class PriorityController : Controller
    {
        private readonly IPriorityService _priorityService;

        public PriorityController(IPriorityService priorityService)
        {
            _priorityService = priorityService;
        }
        [HttpGet("priority")]
        [EnableQuery()]
        public async Task<ActionResult<PriorityLevel>> GetAllTicket()
        {
            var response = await _priorityService.GetAllPriorityAsync();
            return Ok(response);
        }
    }
}
