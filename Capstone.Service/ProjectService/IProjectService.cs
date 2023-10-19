using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;

namespace Capstone.Service.ProjectService;

public interface IProjectService
{
    Task<bool> CreateProject(CreateProjectRequest createProjectRequest);
    Task<bool> CreateProjectRole(CreateRoleRequest createRoleRequest);
	Task<IEnumerable<ViewMemberProject>> GetMemberByProjectId(Guid ProejctId);
	Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid projectId);
	Task<bool> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest);
}