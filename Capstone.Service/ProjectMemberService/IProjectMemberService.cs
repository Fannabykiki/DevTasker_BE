using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.ProjectMember;

namespace Capstone.Service.ProjectMemberService
{
	public interface IProjectMemberService
	{
		Task<AddNewProjectMemberResponse> AddNewProjectMember(InviteUserRequest inviteUserRequest);
	}
}
