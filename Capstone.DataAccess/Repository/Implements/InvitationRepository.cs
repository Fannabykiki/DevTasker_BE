using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
	public class InvitationRepository : BaseRepository<Invitation>, IInvitationRepository
	{
		public InvitationRepository(CapstoneContext context) : base(context)
		{
		}
	}
}
