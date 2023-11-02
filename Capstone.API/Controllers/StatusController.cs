using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.StatusService;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Capstone.API.Controllers
{
    [Route("api/status-management")]
    [ApiController]
    public class StatusController : Controller
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet("status")]
        [EnableQuery()]
        public Task<ActionResult<IQueryable<Status>>> GetAllTicket()
        {
            var response =  _statusService.GetAllStatusAsync();
            return Task.FromResult<ActionResult<IQueryable<Status>>>(Ok(response));
        }
    }
}
