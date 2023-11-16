using Capstone.Common.DTOs.Comments;
using Capstone.DataAccess.Entities;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface ITaskCommentRepository : IBaseRepository<TaskComment>
    {
        Task<List<GetCommentResponse>> GetAllTaskComment(Guid taskId);
    }
}
