using Capstone.API.Extentions;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.TaskPriority;
using Capstone.Common.DTOs.User;
using Capstone.Service.IterationService;
using Capstone.Service.LoggerService;
using Capstone.Service.ProjectService;
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
		private readonly IProjectService _projectService;
		private readonly IIterationService _interationService;

		public TaskController(ILoggerManager logger, ITaskService taskService, IIterationService interationService)
		{
			_logger = logger;
			_taskService = taskService;
			_interationService = interationService;
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

		[HttpGet("tasks/type")]
		public async Task<ActionResult<StatusTaskViewModel>> GetAllTypeTaskByProjectId()
		{
			var response = await _taskService.GetAllTaskType();
			return Ok(response);
		}

		[HttpPost("tasks")]
		public async Task<ActionResult<CreateTaskResponse>> CreateTask(CreateTaskRequest request)
		{
			if (request.InterationId != Guid.Empty)
			{
				var interation = await _interationService.GetIterationsById(request.InterationId);
				if (request.StartDate <= interation.StartDate)
				{
					return BadRequest("Can't create new task with start date before interation's start date. Please update and try again");
				}
				if (request.DueDate >= interation.EndDate)
				{
					return BadRequest("Cant create new task with end date after interation's end date. Please update and try again");
				}
				var userId = this.GetCurrentLoginUserId();
				if (userId == Guid.Empty)
				{
					return BadRequest("You need login first");
				}
				var result = await _taskService.CreateTask(request, userId);
				return Ok(result);
			}
			else
			{
				var interation = await _interationService.GetCurrentInterationId(request.ProjectId);

				if (request.StartDate <= DateTime.Parse(interation.StartDate))
				{
					return BadRequest("Can't create new task with start date before interation's start date. Please update and try again");
				}
				if (request.DueDate >= DateTime.Parse(interation.EndDate))
				{
					return BadRequest("Cant create new task with end date after interation's end date. Please update and try again");
				}
				var userId = this.GetCurrentLoginUserId();
				if (userId == Guid.Empty)
				{
					return BadRequest("You need login first");
				}
				var result = await _taskService.CreateTask(request, userId);
				return Ok(result);
			}
		}

		[HttpPost("tasks/subtask")]
		public async Task<ActionResult<CreateTaskResponse>> CreateSubTask(CreateSubTaskRequest request)
		{
			var interation = await _interationService.GetCurrentInterationId(request.ProjectId);

			if (request.StartDate <= DateTime.Parse(interation.StartDate))
			{
				return BadRequest("Can't create new task with start date before interation's start date. Please update and try again");
			}
			if (request.DueDate >= DateTime.Parse(interation.EndDate))
			{
				return BadRequest("Cant create new task with end date after interation's end date. Please update and try again");
			}
			var userId = this.GetCurrentLoginUserId();
			if (userId == Guid.Empty)
			{
				return BadRequest("You need login first");
			}
			var result = await _taskService.CreateSubTask(request, userId);
			return Ok(result);
		}

		//2
		[HttpPut("tasks")]
		public async Task<IActionResult> Update(UpdateTaskRequest updateTicketRequest)
		{
			var task = await _taskService.CheckExist(updateTicketRequest.TaskId);
			if (!task) {
				return NotFound("Task not found");
			}
			var interation = await _interationService.GetIterationsById(updateTicketRequest.InterationId);
			if (updateTicketRequest.StartDate <= interation.StartDate)
			{
				return BadRequest("Can't create new task with start date before interation's start date. Please update and try again");
			}
			if (updateTicketRequest.DueDate >= interation.EndDate)
			{
				return BadRequest("Cant create new task with end date after interation's end date. Please update and try again");
			}
			var userId = this.GetCurrentLoginUserId();
			if (userId == Guid.Empty)
			{
				return BadRequest("You need login first");
			}
			var result = await _taskService.UpdateTask(updateTicketRequest);
			return Ok(result);
		}
		//3
		[HttpPut("tasks/status")]
		public async Task<IActionResult> UpdateaTaskStastus(UpdateTaskStatusRequest updateTaskStatusRequest)
		{
			var task = await _taskService.CheckExist(updateTaskStatusRequest.TaskId);
			if (!task)
			{
				return NotFound("Task not found");
			}
			var result = await _taskService.UpdateTaskStatus(updateTaskStatusRequest.TaskId, updateTaskStatusRequest);

			return Ok(result);
		}
		//4
		[HttpPut("tasks/deletion")]
		public async Task<IActionResult> DeleteTicket(RestoreTaskRequest restoreTaskRequest)
		{
			var task = await _taskService.CheckExist(restoreTaskRequest.TaskId);
			if (!task)
			{
				return NotFound("Task not found");
			}
			var result = await _taskService.DeleteTask(restoreTaskRequest);

			return Ok(result);
		}
		//1
		[HttpPut("tasks/restoration")]
		public async Task<ActionResult<BaseResponse>> RestoreTask(RestoreTaskRequest restoreTaskRequest)
		{
			var task = await _taskService.CheckExist(restoreTaskRequest.TaskId);
			if (!task)
			{
				return NotFound("Task not found");
			}
			var taskDetail = await _taskService.GetTaskDetail(restoreTaskRequest.TaskId);
			if (taskDetail.DeleteAt == null)
			{
				return BadRequest("Task is still active. Cant restore it!!!");
			}
			if (DateTime.Parse(taskDetail.ExpireTime) >= DateTime.Now)
			{
				var response = await _taskService.RestoreTask(restoreTaskRequest);
				return Ok(response);
			}
			else
			{
				return BadRequest("Cant restore this Task.Over 30 days from delete day");
			}
		}
	}
}
