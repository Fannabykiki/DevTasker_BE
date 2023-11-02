using Capstone.Common.DTOs.Project;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
    public class PriorityRepository : BaseRepository<PriorityLevel>, IPriorityRepository
    {  
        public PriorityRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
