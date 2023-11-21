using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.ProjectMember;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.ProjectMemberService
{
	public interface IProjectMemberService
	{
		Task<BaseResponse> DeclineInvitation(Guid userId, AcceptInviteRequest acceptInviteRequest);
		Task<BaseResponse> AcceptInvitation(Guid userId,AcceptInviteRequest acceptInviteRequest);
		Task<AddNewProjectMemberResponse> AddNewProjectMember(InviteUserRequest inviteUserRequest);
		Task<bool> CheckMemberExist(string email,Guid projectId);
	}
}
