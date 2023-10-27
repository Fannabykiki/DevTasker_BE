using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
	public class SchemaRepository : BaseRepository<Schema>, ISchemaRepository
	{
		private new readonly CapstoneContext _context;
		public SchemaRepository(CapstoneContext context) : base(context)
		{
			_context = context;
		}
		public async Task<List<RoleDTO>> GetPermissionRolesBySchemaId(Guid permissionId, Guid schemaId)
		{
			var role = await _context.SchemaPermissions
				.Where(sp => sp.SchemaId == schemaId && sp.PermissionId == permissionId)
				.Include(sp => sp.Permission)
				.Include(sp => sp.Role)
				.Select(sp => new RoleDTO
				{
					Description = sp.Role.Description,
					RoleId = sp.Role.RoleId,
					RoleName = sp.Role.RoleName,
				})
				.ToListAsync();
			return role;
		}

	}
}

