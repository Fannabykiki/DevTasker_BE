using Capstone.API.Extentions;
using Capstone.API.Extentions.RolePermissionAuthorize;
using Capstone.Common.Constants;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.TaskPriority;
using Capstone.Common.DTOs.User;
using Capstone.Service.IterationService;
using Capstone.Service.LoggerService;
using Capstone.Service.NotificationService;
using Capstone.Service.ProjectService;
using Capstone.Service.TicketService;
using Microsoft.AspNetCore.Authorization;
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
		private readonly IAuthorizationService _authorizationService;
		private readonly INotificationService _notificationService;
		public TaskController(ILoggerManager logger, 
			ITaskService taskService, 
			IIterationService interationService, 
			IProjectService projectService,
            IAuthorizationService authorizationService,
            INotificationService notificationService)
		{
			_logger = logger;
			_taskService = taskService;
			_interationService = interationService;
			_projectService = projectService;
			_authorizationService = authorizationService;
			_notificationService = notificationService;
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
            response.Add(new StatusTaskViewModel { BoardStatusId = Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31"), Title = "Deleted", BoardId = projectId , Order = 0});
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
            //Authorize
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
            new RolePermissionResource
            {
                    ListProjectId = new List<Guid?> { createNewTaskStatus.ProjectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }

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
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { request.ProjectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.CreateTasks }
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }
            var projectStatus = await _projectService.GetProjectByProjectId(request.ProjectId);
			if(projectStatus.StatusId == Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C"))
			{
				return BadRequest("Can't create task in done project");
			}
		 	if (projectStatus.StatusId == Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31"))
			{
				return BadRequest("Can't create task in deleted project");
			}
			var memberStatus = await _projectService.CheckMemberStatus(request.AssignTo);
			if(!memberStatus)
			{
				return BadRequest("Can't assign to unavailable member");
			}
			if (request.InterationId != Guid.Empty)
			{
				var interation = await _interationService.GetIterationsById(request.InterationId);
				if (request.StartDate.Date < interation.StartDate.Date)
				{
					return BadRequest("Can't create new task with start date before sprint's start date. Please update and try again");
				}
				if (request.DueDate.Date > interation.EndDate.Date)
				{
					return BadRequest("Cant create new task with end date after sprint's end date. Please update and try again");
				}
				var userId = this.GetCurrentLoginUserId();
				if (userId == Guid.Empty)
				{
					return BadRequest("You need login first");
				}
				var result = await _taskService.CreateTask(request, userId);
				if (result.BaseResponse.IsSucceed)
				{
                    await _notificationService.SendNotificationCreateTask(result.TaskId, this.GetCurrentLoginUserId());
                }  
                return Ok(result);
			}
			else
			{
				var interation = await _interationService.GetCurrentInterationId(request.ProjectId);

				if (request.StartDate.Date < DateTime.Parse(interation.StartDate).Date)
				{
					return BadRequest("Can't create new task with start date before sprint's start date. Please update and try again");
				}
				if (request.DueDate.Date > DateTime.Parse(interation.EndDate).Date)
				{
					return BadRequest("Cant create new task with end date after sprint's end date. Please update and try again");
				}
				var userId = this.GetCurrentLoginUserId();
				if (userId == Guid.Empty)
				{
					return BadRequest("You need login first");
				}
				var result = await _taskService.CreateTask(request, userId);
                if (result.BaseResponse.IsSucceed)
                {
                    await _notificationService.SendNotificationCreateTask(result.TaskId, this.GetCurrentLoginUserId());
                }
                return Ok(result);
			}
		}

        // 993951AD-5457-41B9-8FFF-4D1C1FA557D0 - Create Tasks
        [HttpPost("tasks/subtask")]
		public async Task<ActionResult<CreateTaskResponse>> CreateSubTask(CreateSubTaskRequest request)
		{
			//Authorize
			var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { request.ProjectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.CreateTasks }
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }
			var projectStatus = await _projectService.GetProjectByProjectId(request.ProjectId);
			if (projectStatus.StatusId == Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C"))
			{
				return BadRequest("Can't create task in done project");
			}
			if (projectStatus.StatusId == Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31"))
			{
				return BadRequest("Can't create task in deleted project");
			}
			var memberStatus = await _projectService.CheckMemberStatus(request.AssignTo);
			if (!memberStatus)
			{
				return BadRequest("Can't assign to unavailable member");
			}
			var task = await _taskService.GetTaskDetail(request.TaskId);
			if (request.StartDate.Date < DateTime.Parse(task.StartDate).Date)
			{
				return BadRequest("Can't create new task with start date before task's start date. Please update and try again");
			}
			if (request.DueDate.Date > DateTime.Parse(task.DueDate).Date)
			{
				return BadRequest("Cant create new task with end date after task's end date. Please update and try again");
			}
			var userId = this.GetCurrentLoginUserId();
			if (userId == Guid.Empty)
			{
				return BadRequest("You need login first");
			}
			var result = await _taskService.CreateSubTask(request, userId);
			if (result.BaseResponse.StatusCode == 400)
			{
				return BadRequest(result.BaseResponse.Message);
			}
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
			//Authorize
			var projectId = await _taskService.GetProjectIdOfTask(updateTicketRequest.TaskId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { projectId},
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.ResolveTasks, 
																PermissionNameConstant.ScheduleTasks,
																PermissionNameConstant.AssignableTasks,
																PermissionNameConstant.EditTasks,
                                                                PermissionNameConstant.AssignTasks}
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }
			var taskDetail = await _taskService.GetTaskDetail(updateTicketRequest.TaskId);
			if (taskDetail == null)
			{
				return NotFound("Task not found");
			}
            var projectStatus = await _projectService.GetProjectByProjectId(taskDetail.ProjectId);
            if (projectStatus.StatusId == Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C"))
			{
				return BadRequest("Can't create subtask in done project");
			}
			if (projectStatus.StatusId == Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31"))
			{
				return BadRequest("Can't create task in deleted project");
			}
			var memberStatus = await _projectService.CheckMemberStatus(updateTicketRequest.AssignTo);
			if (!memberStatus)
			{
				return BadRequest("Can't assign to unavailable member");
			}
			var task = await _taskService.GetTask(updateTicketRequest.TaskId);
			if(task.PrevId != null)
			{
				var taskParent = await _taskService.GetTaskParentDetail(task.PrevId);
				if (updateTicketRequest.StartDate.Date < taskParent.StartDate.Date)
				{
					return BadRequest("Can't update new task with start date before task's start date. Please update and try again");
				}
				if (updateTicketRequest.DueDate.Date > taskParent.DueDate.Date)
				{
					return BadRequest("Can't update new task with end date after task's end date. Please update and try again");
				}
			}
			var interation = await _interationService.GetIterationsById(updateTicketRequest.InterationId);
			if (updateTicketRequest.StartDate.Date < interation.StartDate.Date)
			{
				return BadRequest("Can't create new task with start date before sprint's start date. Please update and try again");
			}
			if (updateTicketRequest.DueDate.Date > interation.EndDate.Date)
			{
				return BadRequest("Can't create new task with end date after sprint's end date. Please update and try again");
			}
			var userId = this.GetCurrentLoginUserId();	
			if (userId == Guid.Empty)
			{
				return BadRequest("You need login first");
			}
			var result = await _taskService.UpdateTask(updateTicketRequest);

            // Notification
            if (result.BaseResponse.IsSucceed)
            {
                await _notificationService.SendNotificationUpdateTask(result.TaskId,this.GetCurrentLoginUserId(), taskDetail);
            }
            return Ok(result);
		}

        //3 A6067E1B-6F37-429C-865C-AA4CC4D829DE - Close Tasks
        [HttpPut("tasks/status")]
		public async Task<IActionResult> UpdateTaskStastus(UpdateTaskStatusRequest updateTaskStatusRequest)
		{
		
			//Authorize
			var projectId = await _taskService.GetProjectIdOfTask(updateTaskStatusRequest.TaskId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { projectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.CloseTasks}
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }
			var projectStatus = await _projectService.GetProjectByProjectId(projectId);
			if (projectStatus.StatusId == Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C"))
			{
				return BadRequest("Can't create task in done project");
			}
			if (projectStatus.StatusId == Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31"))
			{
				return BadRequest("Can't create task in deleted project");
			}
			var task = await _taskService.CheckExist(updateTaskStatusRequest.TaskId);
			if (!task)
			{
				return NotFound("Task not found");
			}
			var result = await _taskService.UpdateTaskStatus(updateTaskStatusRequest.TaskId, updateTaskStatusRequest);
            
            if (result.BaseResponse.IsSucceed)
			{
                await _notificationService.SendNotificationChangeTaskStatus(updateTaskStatusRequest.TaskId, this.GetCurrentLoginUserId());
            }
            return Ok(result);
		}

		[HttpPut("tasks/status/order")]
		public async Task<IActionResult> MoveStastus(UpdateTaskOrderRequest updateTaskOrderRequest)
		{
			//var task = await _taskService.CheckExist(updateTaskOrderRequest.);
			//if (!task)
			//{
			//	return NotFound("Task not found");
			//}
			var result = await _taskService.UpdateTaskOrder(updateTaskOrderRequest);

			return Ok(result);
		}

		[HttpPut("tasks/status/title")]
		public async Task<IActionResult> UpdateStastusName(UpdateTaskNameRequest updateTaskNameRequest)
		{
			//var task = await _taskService.CheckExist(updateTaskOrderRequest.);
			//if (!task)
			//{
			//	return NotFound("Task not found");
			//}
			var result = await _taskService.UpdateTaskTitle(updateTaskNameRequest);

			return Ok(result);
		}

		[HttpPut("tasks/status/deletion")]
		public async Task<IActionResult> DeleteTaskStatus(DeleteTaskStatusRequest deleteTaskStatusRequest)
		{
			//var task = await _taskService.CheckExist(updateTaskOrderRequest.);
			//if (!task)
			//{
			//	return NotFound("Task not found");
			//}
			var result = await _taskService.DeleteTaskStatus(deleteTaskStatusRequest);
            return Ok(result);
		}

		//4 9D7C3592-0CAF-42D1-A7B6-293CA69F6201 - Delete Tasks
		[HttpPut("tasks/deletion")]
		public async Task<IActionResult> DeleteTicket(RestoreTaskRequest restoreTaskRequest)
		{
            //Authorize
            var projectId = await _taskService.GetProjectIdOfTask(restoreTaskRequest.TaskId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { projectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.DeleteTasks}
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }

            var task = await _taskService.CheckExist(restoreTaskRequest.TaskId);
			if (!task)
			{
				return NotFound("Task not found");
			}
			var result = await _taskService.DeleteTask(restoreTaskRequest);
			if (result.IsSucceed)
			{
                await _notificationService.SendNotificationDeleteTask(restoreTaskRequest.TaskId, this.GetCurrentLoginUserId());
            }
            
            return Ok(result);
		}

		[HttpPut("tasks/deletion-each-task")]
		public async Task<IActionResult> DeleteEachTask(RestoreTaskRequest restoreTaskRequest)
		{
			//Authorize
			var projectId = await _taskService.GetProjectIdOfTask(restoreTaskRequest.TaskId);
			var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
				new RolePermissionResource
				{
					ListProjectId = new List<Guid?> { projectId },
					ListPermissionAuthorized = new List<string> { PermissionNameConstant.DeleteTasks }
				}, AuthorizationRequirementNameConstant.RolePermission);
			if (!authorizationResult.Succeeded)
			{
				return Unauthorized(ErrorMessage.InvalidPermission);
			}

			var task = await _taskService.CheckExist(restoreTaskRequest.TaskId);
			if (!task)
			{
				return NotFound("Task not found");
			}
			var result = await _taskService.DeleteEachTask(restoreTaskRequest);
			if (result.IsSucceed)
			{
				await _notificationService.SendNotificationDeleteTask(restoreTaskRequest.TaskId, this.GetCurrentLoginUserId());
			}

			return Ok(result);
		}

		//1 E83C8597-8181-424A-B48F-CA3A8AA021B1 - Administer Projects
		[HttpPut("tasks/restoration")]
		public async Task<ActionResult<BaseResponse>> RestoreTask(RestoreTaskRequest restoreTaskRequest)
		{
			//Authorize
			var projectId = await _taskService.GetProjectIdOfTask(restoreTaskRequest.TaskId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
                new RolePermissionResource
                {
                    ListProjectId = new List<Guid?> { projectId },
                    ListPermissionAuthorized = new List<string> { PermissionNameConstant.AdministerProjects }
                }, AuthorizationRequirementNameConstant.RolePermission);
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized(ErrorMessage.InvalidPermission);
            }
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
			var status = await _taskService.CheckTaskStatus(taskDetail.StatusId);
			if (!status)
			{
				return BadRequest($"Can't restore task because {taskDetail.StatusName} column has been removed ");
			}

			if (DateTime.Parse(taskDetail.ExpireTime).Date > DateTime.Now.Date)
			{
				var response = await _taskService.RestoreTask(restoreTaskRequest);
				if (response.IsSucceed)
				{
					await _notificationService.SendNotificationRestoreTask(restoreTaskRequest.TaskId, this.GetCurrentLoginUserId());
				}
				return Ok(response);
			}
			else
			{
				return BadRequest("Can't restore this Task.Over 30 days from delete day");
			}
		}
	}
}
