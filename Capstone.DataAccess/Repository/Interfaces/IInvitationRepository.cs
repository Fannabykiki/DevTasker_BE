using Capstone.Common.DTOs.Invitaion;
using Capstone.DataAccess.Entities;

namespace Capstone.DataAccess.Repository.Interfaces
{
	public interface IInvitationRepository : IBaseRepository<Invitation>
	{
	Task<InvitationResponse> GetInvitation(Guid invationId);
	}
}
