using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<GetProjectReportRequest> GetProjectReport(Guid projectId);
    }
}
