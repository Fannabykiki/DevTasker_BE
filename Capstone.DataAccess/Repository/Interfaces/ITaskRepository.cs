using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface ITaskRepository : IBaseRepository<Entities.Task>
    {
        Task<List<TaskViewModel>> GetAllTask(Guid projectId);
        Task<List<TaskViewModel>> GetTaskByInterationId(Guid interationId);
        Task<List<TaskViewModel>> GetAllTaskCompleted(Guid projectId,Guid statusId);
        Task<List<TaskViewModel>> GetAllTaskDelete(Guid projectId);
	    Task<TaskDetailViewModel> GetTaskDetail(Guid taskId);
		Task<int> GetTaskDone(Guid projectId);
		Task<int> GetTotalTask(Guid projectId);
	}
}
