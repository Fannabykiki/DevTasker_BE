using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.ProjectMember;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.ProjectMemberService
{
	public interface IProjectMemberService
	{
		Task<BaseResponse> DeclineInvitation(Guid userId,Guid projectId);
		Task<BaseResponse> AcceptInvitation(Guid userId,Guid projectId);
		Task<AddNewProjectMemberResponse> AddNewProjectMember(InviteUserRequest inviteUserRequest);
	}
}
