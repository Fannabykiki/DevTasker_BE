using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
	public class InteratationRepository : BaseRepository<Interation>, IInterationRepository
	{
		public InteratationRepository(CapstoneContext context) : base(context)
		{
		}
	}
}
