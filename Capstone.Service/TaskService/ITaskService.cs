using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;
using Task = Capstone.DataAccess.Entities.Task;

namespace Capstone.Service.TicketService
{
    public interface ITaskService
    {
        Task<bool> CreateTask(CreateTaskRequest createTicketRequest, Guid interationId,Guid userId, Guid projectId);
        Task<bool> UpdateTask(UpdateTaskRequest updateTicketRequest, Guid ticketId);
        Task<IQueryable<Task>> GetAllTaskAsync();
        Task<IQueryable<Task>> GetAllTaskByInterationIdAsync(Guid interationId);
        Task<bool> DeleteTask(Guid ticketId);
    }
}
