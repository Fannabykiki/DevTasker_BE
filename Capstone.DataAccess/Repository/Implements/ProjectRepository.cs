using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        private new readonly CapstoneContext _context;
        public ProjectRepository(CapstoneContext context) : base(context)
        {
            _context = context;
        }

	}
    
}
