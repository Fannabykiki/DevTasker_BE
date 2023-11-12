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

        [HttpGet("kanban-task")]
        [EnableQuery()]
        public async Task<ActionResult<UserResponse>> GetAllTask(Guid projetcId)
        {
            var response = await _taskService.GetAllTaskAsync(projetcId);
            return Ok(response);
        }
        
        [HttpGet("task/{task}")]
        [EnableQuery()]
        public async Task<ActionResult<UserResponse>> GetAllTaskByInterationId(Guid interationId)
        {
            var response = await _taskService.GetAllTaskByInterationIdAsync(interationId);
            return Ok(response);
        }

		[HttpGet("task-status")]
		[EnableQuery()]
		public async Task<ActionResult<List<StatusTaskViewModel>>> GetAllStatusTaskByProjectId(Guid projectId)
		{
			var response = await _taskService.GetAllTaskStatus(projectId);
			return Ok(response);
		}

		[HttpPost("task-status")]
		public async Task<ActionResult<StatusTaskViewModel>> CreateNewStatus(CreateNewTaskStatus createNewTaskStatus)
		{
			var response = await _taskService.CreateTaskStatus(createNewTaskStatus);
			return Ok(response);
		}

		[HttpGet("task-type")]
		public async Task<ActionResult<StatusTaskViewModel>> GetAllTypeTaskByProjectId()
		{
			var response = await _taskService.GetAllTaskType();
			return Ok(response);
		}

		[HttpPost("task")]
        public async Task<ActionResult<CreateTaskResponse>> CreateTask(CreateTaskRequest request)
        {   
            var userId = this.GetCurrentLoginUserId();
            var result = await _taskService.CreateTask(request, userId);

            return Ok(result);
        }

		[HttpPost("subtask")]
		public async Task<ActionResult<CreateTaskResponse>> CreateSubTask(CreateSubTaskRequest request)
		{
			var userId = this.GetCurrentLoginUserId();
			var result = await _taskService.CreateSubTask(request, userId);

			return Ok(result);
		}

		[HttpPut("task/{taskId}")]
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
