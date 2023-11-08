using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.PermissionSchemaService
{
    public interface IPermissionSchemaService
    {
        Task<GetAllPermissionSchemaResponse> GetAllSchema();
        Task<GetSchemaResponse> GetSchemaById(Guid SchemaId);
        Task<GetPermissionSchemaByIdResponse> GetPermissionSchemaById(Guid SchemaId);
        Task<bool> CreateNewPermissionSchema(CreateNewSchemaRequest createNewSchemaRequest);
        Task<bool> UpdateSchema(Guid schemaId, UpdateSchemaRequest updateSchemaRequest);
        Task<bool> GrantSchemaPermissionRoles(Guid schemaId, GrantPermissionSchemaRequest grantPermissionSchemaRequest);
        Task<bool> RevokeSchemaPermissionRoles(Guid schemaId, RevokePermissionSchemaRequest revokePermissionSchemaRequest);
    }
}
