using Capstone.Common.DTOs.Project;

namespace Capstone.Service.ProjectService;

public interface IProjectService
{
    Task<bool> CreateProject(CreateProjectRequest createProjectRequest);
    Task<bool> CreateProjectRole(CreateRoleRequest createRoleRequest);
	Task<IEnumerable<ViewMemberProject>> GetMemberByProjectId(Guid ProejctId);
	Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid projectId);

}