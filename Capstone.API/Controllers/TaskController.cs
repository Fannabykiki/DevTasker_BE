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

		public TaskController(ILoggerManager logger, ITaskService taskService, IIterationService interationService, IProjectService projectService)
		{
			_logger = logger;
			_taskService = taskService;
			_interationService = interationService;
			_projectService = projectService;
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

        // E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
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

        //  993951AD-5457-41B9-8FFF-4D1C1FA557D0 - Create Tasks
        [HttpPost("tasks")]
		public async Task<ActionResult<CreateTaskResponse>> CreateTask(CreateTaskRequest request)
		{
			var memberStatus = await _projectService.CheckMemberStatus(request.AssignTo);
			if(!memberStatus)
			{
				return BadRequest("Can't assign to unavailable member");
			}
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

        // 993951AD-5457-41B9-8FFF-4D1C1FA557D0 - Create Tasks
        [HttpPost("tasks/subtask")]
		public async Task<ActionResult<CreateTaskResponse>> CreateSubTask(CreateSubTaskRequest request)
		{
			var memberStatus = await _projectService.CheckMemberStatus(request.AssignTo);
			if (!memberStatus)
			{
				return BadRequest("Can't assign to unavailable member");
			}
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

        //2  3C815EC0-267E-4054-A9F4-052BE036F3E0 - Resolve Tasks
        //   7E5B494A-9408-4E51-BC8E-3E8E691DE567 - Schedule Tasks
        //   8CA7772E-F397-47FE-AB09-4D53D4A8815D - Assignable User
        //   3B2F8222-D69C-4E45-B1B5-5FF0412BF3D9 - Edit Tasks
        //   7594A882-7833-4B65-9F3E-B27F8A7DCA64 - Assign tasks
        //   10486DAB-608D-4CF5-813E-E6DCD10F43F9 - Edit Tasks
        [HttpPut("tasks")]
		public async Task<IActionResult> Update(UpdateTaskRequest updateTicketRequest)
		{
			var memberStatus = await _projectService.CheckMemberStatus(updateTicketRequest.AssignTo);
			if (!memberStatus)
			{
				return BadRequest("Can't assign to unavailable member");
			}
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

        //3 A6067E1B-6F37-429C-865C-AA4CC4D829DE - Close Tasks
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

        //4 9D7C3592-0CAF-42D1-A7B6-293CA69F6201 - Delete Tasks
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
        //1 E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
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
