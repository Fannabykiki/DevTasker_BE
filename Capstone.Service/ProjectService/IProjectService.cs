﻿using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Invitaion;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Paging;
using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.ProjectService;

public interface IProjectService
{
	Task<IQueryable<GetAllProjectResponse>> GetProjectsAdmin();
	Task<GetProjectReportRequest> GetProjectReport(Guid projectId);
	Task<List<GetProjectTasksResponse>> GetProjectsTasks(Guid projectId);
	Task<IEnumerable<GetUserProjectAnalyzeResponse>> GetUserProjectAnalyze(Guid userId);
	Task<List<GetProjectCalendarResponse>> GetProjectCalender(Guid projectId);
    Task<ProjectAnalyzeRespone> ProjectAnalyzeAdmin();
    Task<CreateProjectRespone> CreateProject(CreateProjectRequest createProjectRequest, Guid userId);
    Task<BaseResponse> CreateProjectRole(CreateRoleRequest createRoleRequest);
	Task<IEnumerable<ViewMemberProject>> GetMemberByProjectId(Guid projectId);
	Task<IEnumerable<GetAllProjectViewModel>> GetProjectByUserId(Guid projectId);
	Task<BaseResponse> UpdateMemberRole(Guid memberId, UpdateMemberRoleRequest updateMemberRoleRequest, Guid updateBy);
	Task<BaseResponse> UpdateProjectInfo(Guid projectId, UpdateProjectNameInfo updateProjectNameInfo);
	Task<BaseResponse> UpdateProjectSchema(Guid projectId, UpdatePermissionSchemaRequest changePermissionSchemaRequest);
	Task<BaseResponse> UpdateProjectPrivacy(Guid projectId, UpdateProjectPrivacyRequest updateProjectPrivacyRequest);
	Task<BaseResponse> DeleteProject(Guid projectId);
	Task<BaseResponse> RestoreProject(Guid projectId);
	Task<GetAllProjectViewModel> GetProjectByProjectId(Guid? projectId );
	Task<IEnumerable<PermissionViewModel>> GetPermissionByUserId(Guid projectId,Guid userId);
    Task<IEnumerable<PermissionViewModel>> GetPermissionAuthorizeByUserId(Guid projectId, Guid userId);
    Task<ViewProjectInfoRequest> GetInfoProjectByProjectId(Guid projectId);
	Task<bool?> SendMailInviteUser(InviteUserRequest inviteUserRequest,Guid userId);
	Task<List<InterationViewModel>> GetInterationByProjectId(Guid projectId);
	Task<BaseResponse> RemoveProjectMember(Guid memberId);
	Task<BaseResponse> ChangeProjectSchema(Guid projectId);
	Task<BaseResponse> ExitProject(Guid userId, Guid projectId);
	Task<ChangeProjectStatusRespone> ChangeProjectStatus(Guid statusId,ChangeProjectStatusRequest changeProjectStatusRequest);
	Task<InvitationResponse> CheckInvation(Guid invationId);
	Task<List<ProjectStatusViewModel>> GetAllProjectStatus(Guid projectId);
	Task<int> GetTaskStatusDone(Guid projectId);
	Task<bool> CheckExist(Guid projectId);
	Task<bool> CheckMemberStatus(Guid memberId);
}