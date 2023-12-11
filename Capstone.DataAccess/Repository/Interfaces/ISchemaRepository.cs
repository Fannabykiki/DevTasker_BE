using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.DataAccess.Repository.Interfaces
{
	public interface ISchemaRepository : IBaseRepository<Schema>
	{
		Task<List<RoleDTO>> GetPermissionRolesBySchemaId(Guid permissionId, Guid schemaId);
		Task<bool> DeleteSchemaById(Guid schemaId);
	}
}
