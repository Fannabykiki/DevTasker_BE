using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.TicketService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly ITicketService _ticketService;

        public TicketController(ILoggerManager logger, ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;
        }

       /* [HttpPost("Ticket")]
        public async Task<IActionResult> CreateProject(CreateTicketRequest createTicketRequest)
        {
            var result = await _ticketService.CreateTicket(createTicketRequest);

            return Ok(result);
        }*/
    }
}
