﻿using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.Ticket;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.TicketService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

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

        [HttpGet("Ticket")]
        [EnableQuery()]
        public async Task<ActionResult<ViewPagedUsersResponse>> GetUsers()
        {
            var response = await _ticketService.GetAllTicketAsync();
            return Ok(response);
        }
        
        [HttpPost("Ticket")]
        public async Task<IActionResult> CreateTicket(CreateTicketRequest createTicketRequest, Guid interationId)
        {
            var result = await _ticketService.CreateTicket(createTicketRequest, interationId);

            return Ok(result);
        }

        [HttpPut("Ticket/{ticketId}")]
        public async Task<IActionResult> UpdateTicket(UpdateTicketRequest updateTicketRequest, Guid ticketId)
        {
            var result = await _ticketService.UpdateTicket(updateTicketRequest, ticketId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}
