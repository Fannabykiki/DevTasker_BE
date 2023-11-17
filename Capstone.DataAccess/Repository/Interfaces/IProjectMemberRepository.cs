using Capstone.DataAccess.Entities;
using static System.Reflection.Metadata.BlobBuilder;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface IProjectMemberRepository : IBaseRepository<ProjectMember>
    {
		public Task<List<ProjectMember>> GetProjectMembers(Guid projectId);
		public Task<List<ProjectMember>> GetProjectByUserId(Guid userId);

	}
}
