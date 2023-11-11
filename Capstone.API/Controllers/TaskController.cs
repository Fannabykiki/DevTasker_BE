using Capstone.API.Extentions;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.TicketService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Capstone.API.Controllers
{
    [Route("api/task-management")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly ITaskService _taskService;

        public TaskController(ILoggerManager logger, ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        [HttpGet("task")]
        [EnableQuery()]
        public async Task<ActionResult<UserResponse>> GetAllTask()
        {
            var response = await _taskService.GetAllTaskAsync();
            return Ok(response);
        }
        
        [HttpGet("task/{task}")]
        [EnableQuery()]
        public async Task<ActionResult<UserResponse>> GetAllTaskByInterationId(Guid interationId)
        {
            var response = await _taskService.GetAllTaskByInterationIdAsync(interationId);
            return Ok(response);
        }
        
        [HttpPost("task")]
        public async Task<ActionResult<CreateTaskResponse>> CreateTask(CreateTaskRequest createTaskRequest, Guid interationId, Guid projectId,Guid statusId)
        {   
            var userId = this.GetCurrentLoginUserId();
            var result = await _taskService.CreateTask(createTaskRequest, interationId,userId, projectId,statusId);

            return Ok(result);
        }

        [HttpPut("task/{tasktId}")]
        public async Task<IActionResult> UpdateaTask(UpdateTaskRequest updateTicketRequest, Guid ticketId)
        {
            var result = await _taskService.UpdateTask(updateTicketRequest, ticketId);

            return Ok(result);
        }
        
        [HttpPut("task/delete/{taskId}")]
        public async Task<IActionResult> DeleteTicket(Guid ticketId)
        {
            var result = await _taskService.DeleteTask( ticketId);

            return Ok(result);
        }
    }
}
