using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;
using Task = Capstone.DataAccess.Entities.Task;

namespace Capstone.Service.TicketService
{
    public interface ITaskService
    {
        Task<CreateTaskResponse> CreateTask(CreateTaskRequest createTicketRequest, Guid interationId,Guid userId, Guid projectId,Guid statusId);
        Task<bool> UpdateTask(UpdateTaskRequest updateTicketRequest, Guid ticketId);
        Task<IQueryable<Task>> GetAllTaskAsync();
        Task<IQueryable<Task>> GetAllTaskByInterationIdAsync(Guid interationId);
        Task<bool> DeleteTask(Guid ticketId);
    }
}
