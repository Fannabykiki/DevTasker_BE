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
        Task<CreateTaskResponse> UpdateTask(Guid taskId, UpdateTaskRequest updateTicketRequest);
        Task<CreateTaskResponse> UpdateTaskStatus(Guid taskId, UpdateTaskStatusRequest updateTaskStatusRequest);
        Task<List<TaskViewModel>> GetAllTaskAsync(Guid projectId);
        Task<IQueryable<Task>> GetAllTaskByInterationIdAsync(Guid interationId);
        Task<BaseResponse> DeleteTask(Guid ticketId);
		Task<StatusTaskViewModel>  CreateTaskStatus(CreateNewTaskStatus createNewTaskStatus);
		Task<List<StatusTaskViewModel>> GetAllTaskStatus(Guid projectId);
		Task<List<TaskTypeViewModel>> GetAllTaskType();
		Task<List<GetAllTaskPriority>> GetAllTaskPriotiry();
		Task<BaseResponse> RestoreTask(Guid taskId);
		Task<List<TaskViewModel>> GetAllTaskDeleteAsync(Guid projetcId);
		Task<TaskDetailViewModel> GetTaskDetail(Guid taskId);
	}
}
