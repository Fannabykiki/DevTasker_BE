﻿using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
    public class TicketHistoryRepository : BaseRepository<TaskHistory>, ITicketHistoryRepository
    {
        public TicketHistoryRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
