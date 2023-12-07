using Capstone.Common.DTOs.Permission;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
    public class PermissionSchemaRepository : BaseRepository<SchemaPermission>, IPermissionSchemaRepository
    {
        public PermissionSchemaRepository(CapstoneContext context) : base(context)
        {
        }

		public async Task<List<PermissionViewModel>> GetPermissionByUserId(Guid? roleId)
		{
			var role = await _context.SchemaPermissions
				.Where(sp => sp.RoleId == roleId)
				.Include(sp => sp.Permission)
				.Select(sp => new PermissionViewModel
				{
					Description = sp.Permission.Description,
					Name = sp.Permission.Name,
					PermissionId = sp.Permission.PermissionId
				})
				.ToListAsync();
			return role;
		}
        public async Task<List<PermissionViewModel>> GetPermissionBySchewmaAndRoleId(Guid? schemaId, Guid? roleId)
		{
            var role = await _context.SchemaPermissions
                .Where(sp => sp.RoleId == roleId && sp.SchemaId == schemaId)
                .Include(sp => sp.Permission)
                .Select(sp => new PermissionViewModel
                {
                    Description = sp.Permission.Description,
                    Name = sp.Permission.Name,
                    PermissionId = sp.Permission.PermissionId
                })
                .ToListAsync();
            return role;
        }
    }
}
