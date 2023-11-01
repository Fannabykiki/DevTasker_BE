using Capstone.Common.DTOs.Project;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
    public class PriorityRepository : BaseRepository<PriorityLevel>, IPriorityRepository
    {private new readonly CapstoneContext _context;
        public PriorityRepository(CapstoneContext context) : base(context)
        {
            _context = context;
        }

        // public async Task<List<ViewProjectInfoRequest>> GetInfoProjectByProjectId(Guid projectId)
        // {
        //     var projectInfoRequests = new List<ViewProjectInfoRequest>();
        //     var project = await _context.Projects.Where(x=>x.ProjectId==projectId).Include(x=>x.ProjectMembers).fi;
        //     var projectInfoRequest = new ViewProjectInfoRequest
        //     {
        //         ProjectId = project.ProjectId,
        //         ProjectName = project.ProjectName,
        //         Description = project.Description,
        //         ProjectStatus = project.ProjectStatus,
        //         StartDate = project.StartDate,
        //         EndDate = project.EndDate,
        //         CreateBy = project.CreateBy,
        //         CreateAt = project.CreateAt,
        //         PrivacyStatus = project.PrivacyStatus,
        //         ProjectMembers = project.ProjectMembers
        //             .Select(member => new ViewMemberProject
        //             {
        //                 MemberId = member.MemberId,
        //                 UserId = member.UserId,
        //                 RoleId = member.RoleId,
        //                 ProjectId = member.ProjectId,
        //                 IsOwner = member.IsOwner
        //                 ,RoleName = member.Role.RoleName
					   //
        //             })
        //             .ToList()
        //     };
        //
        //     projectInfoRequests.Add(projectInfoRequest);
        //
        //     return projectInfoRequests;
        // }
    }
}
