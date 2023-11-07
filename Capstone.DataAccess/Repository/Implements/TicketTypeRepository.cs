﻿using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
    public class TicketTypeRepository : BaseRepository<TaskType>, ITicketTypeRepository
    {
        public TicketTypeRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
