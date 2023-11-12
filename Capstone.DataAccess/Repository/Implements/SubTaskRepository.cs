using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
	public class SubTaskRepository : BaseRepository<SubTask>, ISubTaskRepository
	{
		public SubTaskRepository(CapstoneContext context) : base(context)
		{
		}
	}
}
