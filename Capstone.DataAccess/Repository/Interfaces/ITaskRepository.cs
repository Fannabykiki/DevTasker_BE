using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface ITaskRepository : IBaseRepository<Entities.Task>
    {
        Task<List<TaskViewModel>> GetAllTask(Guid projectId);
    }
}
