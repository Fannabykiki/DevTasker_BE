using Capstone.Common.DTOs.Project;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
    public class ProjectMemberRepository : BaseRepository<ProjectMember>, IProjectMemberRepository
    {
        public ProjectMemberRepository(CapstoneContext context) : base(context)
        {
        }

		public async Task<List<ProjectMember>> GetProjectMembers(Guid projectId)
		{
            var projectMember = await _context.ProjectMembers.Where(x => x.ProjectId == projectId).Include(x => x.Users).Include(x => x.Role).Include(x=>x.Status).ToListAsync();
            return projectMember;
		}

		public async Task<List<ProjectMember>> GetProjectByUserId(Guid userId)
		{
			var projectMember = await _context.ProjectMembers.Where(x => x.UserId == userId).Include(x => x.Users).Include(x => x.Role).Include(x => x.Status).Include(x =>x.Project).ThenInclude(x=>x.Status).ToListAsync();
			return projectMember;
		}

		public async Task<List<ProjectMember>> CheckStatus(Guid projectId,Guid statusId)
		{
			var projectMember = await _context.ProjectMembers.Where(x => x.ProjectId == projectId && x.StatusId == statusId).Include(x => x.Users).Include(x => x.Role).Include(x => x.Status).ToListAsync();
			return projectMember;
		}
	}
}
