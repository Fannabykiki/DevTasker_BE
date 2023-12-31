﻿using Capstone.Common.DTOs.Permission;
using Capstone.DataAccess.Entities;
using static System.Reflection.Metadata.BlobBuilder;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface IPermissionSchemaRepository : IBaseRepository<SchemaPermission>
    {
		Task<List<PermissionViewModel>> GetPermissionByUserId(Guid? roleId);
        Task<List<PermissionViewModel>> GetPermissionBySchewmaAndRoleId(Guid? schemaId, Guid? roleId);
    }
}
