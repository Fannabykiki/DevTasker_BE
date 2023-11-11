using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements;

public class BoardStatusRepository : BaseRepository<BoardStatus>, IBoardStatusRepository
{
    public BoardStatusRepository(CapstoneContext context) : base(context)
    {
    }
}
