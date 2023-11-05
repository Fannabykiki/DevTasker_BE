using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;

namespace Capstone.Service.ProjectService;

public interface IProjectService
{
    Task<CreateProjectRespone> CreateProject(CreateProjectRequest createProjectRequest, Guid userId);
    Task<bool> CreateProjectRole(CreateRoleRequest createRoleRequest);
	Task<IEnumerable<ViewMemberProject>> GetMemberByProjectId(Guid projectId);
	Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid projectId);
	Task<bool> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest);
	Task<bool> UpdateProjectInfo(Guid projectId, UpdateProjectNameInfo updateProjectNameInfo);
	Task<bool> UpdateProjectPrivacy(Guid projectId, UpdateProjectPrivacyRequest updateProjectPrivacyRequest);
	Task<bool> DeleteProject(Guid projectId);
	Task<bool> RestoreProject(Guid projectId);
	Task<GetAllProjectViewModel> GetProjectByProjectId(Guid projectId);
	Task<IEnumerable<PermissionViewModel>> GetPermissionByUserId(Guid projectId,Guid userId);
	Task<List<ViewProjectInfoRequest>> GetInfoProjectByProjectId(Guid projectId);
}