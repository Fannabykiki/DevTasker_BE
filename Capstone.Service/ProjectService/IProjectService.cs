using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.ProjectService;

public interface IProjectService
{
    Task<bool> CreateProject(CreateProjectRequest createProjectRequest);
    Task<bool> CreateProjectRole(CreateRoleRequest createRoleRequest);
	Task<ListViewMemberProject> GetMemberByProjectId(Guid projectId);
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