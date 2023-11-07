using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
    public class TicketRepository : BaseRepository<Entities.Task>, ITicketRepository
    {
        public TicketRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
