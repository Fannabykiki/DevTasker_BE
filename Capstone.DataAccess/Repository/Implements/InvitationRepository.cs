using Capstone.Common.DTOs.Invitaion;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
	public class InvitationRepository : BaseRepository<Invitation>, IInvitationRepository
	{
		public InvitationRepository(CapstoneContext context) : base(context)
		{
		}

		public async Task<InvitationResponse> GetInvitation(Guid invationId)
		{
			var result = await _context.Invitations.Where(x=>x.InvitationId == invationId).Include(x=>x.Status).Include(x=>x.ProjectMember).ThenInclude(x=>x.Users).FirstOrDefaultAsync();
			return new InvitationResponse
			{
				CreateAt = result.CreateAt,
				InviteBy = result.ProjectMember.Users.UserName,
				InvitationId = result.InvitationId,
				InviteTo = result.InviteTo,
				StatusId = result.StatusId,
				StatusName = result.Status.Title,
				ProjectId = result.ProjectId,
				ProjectName = result.ProjectName
			};
		}
	}
}
