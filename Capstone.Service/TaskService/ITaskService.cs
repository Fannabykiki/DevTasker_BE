using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;
using Task = Capstone.DataAccess.Entities.Task;

namespace Capstone.Service.TicketService
{
    public interface ITaskService
    {
        Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, Guid userId);
        Task<CreateTaskResponse> CreateSubTask(CreateSubTaskRequest request, Guid userId);
        Task<bool> UpdateTask(UpdateTaskRequest updateTicketRequest, Guid ticketId);
        Task<List<TaskViewModel>> GetAllTaskAsync(Guid projectId);
        Task<IQueryable<Task>> GetAllTaskByInterationIdAsync(Guid interationId);
        Task<bool> DeleteTask(Guid ticketId);
		Task<StatusTaskViewModel>  CreateTaskStatus(CreateNewTaskStatus createNewTaskStatus);
		Task<List<StatusTaskViewModel>> GetAllTaskStatus(Guid projectId);
		Task<List<TaskTypeViewModel>> GetAllTaskType();
	}
}
