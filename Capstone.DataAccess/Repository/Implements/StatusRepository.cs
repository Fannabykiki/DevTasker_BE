using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
    public class StatusRepository : BaseRepository<Status>, IStatusRepository
    {
        public StatusRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
