using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.TaskPriority;
using Capstone.DataAccess.Entities;
using Task = Capstone.DataAccess.Entities.Task;

namespace Capstone.Service.TicketService
{
    public interface ITaskService
    {
        Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, Guid userId);
        Task<CreateTaskResponse> CreateSubTask(CreateSubTaskRequest request, Guid userId);
        Task<CreateTaskResponse> UpdateTask(UpdateTaskRequest updateTicketRequest);
        Task<CreateTaskResponse> UpdateTaskStatus(Guid taskId, UpdateTaskStatusRequest updateTaskStatusRequest);
        Task<List<TaskViewModel>> GetAllTaskAsync(Guid projectId);
        Task<IQueryable<Task>> GetAllTaskByInterationIdAsync(Guid interationId);
        Task<BaseResponse> DeleteTask(RestoreTaskRequest restoreTaskRequest);
        Task<BaseResponse> DeleteEachTask(RestoreTaskRequest restoreTaskRequest);
		Task<StatusTaskViewModel>  CreateTaskStatus(CreateNewTaskStatus createNewTaskStatus);
		Task<List<StatusTaskViewModel>> GetAllTaskStatus(Guid projectId);
		Task<List<TaskTypeViewModel>> GetAllTaskType();
		Task<List<GetAllTaskPriority>> GetAllTaskPriotiry();
		Task<BaseResponse> RestoreTask(RestoreTaskRequest restoreTaskRequest);
		Task<List<TaskViewModel>> GetAllTaskDeleteAsync(Guid projetcId);
		Task<TaskDetailViewModel> GetTaskDetail(Guid taskId);
        Task<Guid?> GetProjectIdOfTask(Guid taskId);
		Task<bool> CheckExist(Guid taskId);
		Task<UpdateTaskOrderResponse> UpdateTaskOrder(UpdateTaskOrderRequest updateTaskOrderRequest);
		Task<UpdateTaskOrderResponse> UpdateTaskTitle(UpdateTaskNameRequest updateTaskNameRequest);
		Task<BaseResponse> DeleteTaskStatus(DeleteTaskStatusRequest deleteTaskStatusRequest);
		Task<bool> CheckTaskStatus(Guid taskId);
	}
}
