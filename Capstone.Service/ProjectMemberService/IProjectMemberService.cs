using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.ProjectMember;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.ProjectMemberService
{
	public interface IProjectMemberService
	{
		Task<bool> AcceptInvitation(Guid userId,Guid projectId);
		Task<AddNewProjectMemberResponse> AddNewProjectMember(InviteUserRequest inviteUserRequest);
	}
}
