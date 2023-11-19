using Capstone.API.Extentions;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.TaskPriority;
using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.TicketService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.CodeAnalysis;

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

        [HttpGet("tasks/kanban")]
        [EnableQuery()]
        public async Task<ActionResult<List<TaskViewModel>>> GetAllTask(Guid projetcId)
        {
            var response = await _taskService.GetAllTaskAsync(projetcId);
            return Ok(response);
        }

		[HttpGet("tasks/task-bin")]
		[EnableQuery()]
		public async Task<ActionResult<List<TaskViewModel>>> GetAllTaskDelete(Guid projetcId)
		{
			var response = await _taskService.GetAllTaskDeleteAsync(projetcId);
			return Ok(response);
		}

		[HttpGet("tasks/detail")]
		[EnableQuery()]
		public async Task<ActionResult<TaskDetailViewModel>> GetTaskDetail(Guid taskId)
		{
			var response = await _taskService.GetTaskDetail(taskId);
			return Ok(response);
		}

		[HttpGet("task/{task}")]
        [EnableQuery()]
        public async Task<ActionResult<UserResponse>> GetAllTaskByInterationId(Guid interationId)
        {
            var response = await _taskService.GetAllTaskByInterationIdAsync(interationId);
            return Ok(response);
        }

		[HttpGet("tasks/status")]
		[EnableQuery()]
		public async Task<ActionResult<List<StatusTaskViewModel>>> GetAllStatusTaskByProjectId(Guid projectId)
		{
			var response = await _taskService.GetAllTaskStatus(projectId);
			return Ok(response);
		}

		[HttpGet("tasks/priority")]
		[EnableQuery()]
		public async Task<ActionResult<List<GetAllTaskPriority>>> GetAllTaskPriotiry()
		{
			var response = await _taskService.GetAllTaskPriotiry();
			return Ok(response);
		}


		[HttpPost("tasks/status")]
		public async Task<ActionResult<StatusTaskViewModel>> CreateNewStatus(CreateNewTaskStatus createNewTaskStatus)
		{
			var response = await _taskService.CreateTaskStatus(createNewTaskStatus);
			return Ok(response);
		}

		[HttpPut("tasks/restoration")]
		public async Task<ActionResult<BaseResponse>> RestoreTask(Guid taskId)
		{
			
			var task = await _taskService.GetTaskDetail(taskId);
			if (task.DeleteAt == null)
			{
				return BadRequest("Task is still active. Cant restore it!!!");
			}
			if (DateTime.Parse(task.ExpireTime) >= DateTime.Now)
			{
				var response = await _taskService.RestoreTask(taskId);
				return Ok(response);
			}
			else
			{
				return BadRequest("Cant restore this Task.Over 30 days from delete day");
			}
		}


		[HttpGet("tasks/type")]
		public async Task<ActionResult<StatusTaskViewModel>> GetAllTypeTaskByProjectId()
		{
			var response = await _taskService.GetAllTaskType();
			return Ok(response);
		}

		[HttpPost("tasks")]
        public async Task<ActionResult<CreateTaskResponse>> CreateTask(CreateTaskRequest request)
        {   
            var userId = this.GetCurrentLoginUserId();
            if(userId == Guid.Empty)
            {
                return BadRequest("You need login first");
            }
            var result = await _taskService.CreateTask(request, userId);

            return Ok(result);
        }

		[HttpPost("tasks/subtask")]
		public async Task<ActionResult<CreateTaskResponse>> CreateSubTask(CreateSubTaskRequest request)
		{
			var userId = this.GetCurrentLoginUserId();
			if (userId == null)
			{
				return BadRequest("You need login first");
			}
			var result = await _taskService.CreateSubTask(request, userId);

			return Ok(result);
		}

		[HttpPut("tasks/{taskId}")]
        public async Task<IActionResult> UpdateaTask(UpdateTaskRequest updateTicketRequest)
        {
            var result = await _taskService.UpdateTask(updateTicketRequest);

            return Ok(result);
        }
		
		[HttpPut("tasks/change-status/{taskId}")]
        public async Task<IActionResult> UpdateaTaskStastus(Guid taskId, UpdateTaskStatusRequest updateTaskStatusRequest)
        {
            var result = await _taskService.UpdateTaskStatus(taskId, updateTaskStatusRequest);

            return Ok(result);
        }
        
        [HttpPut("task/deletion/{taskId}")]
        public async Task<IActionResult> DeleteTicket(Guid ticketId)
        {
            var result = await _taskService.DeleteTask( ticketId);

            return Ok(result);
        }
    }
}
