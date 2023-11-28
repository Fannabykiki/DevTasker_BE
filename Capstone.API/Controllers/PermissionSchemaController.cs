using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Role;
using Capstone.Common.DTOs.Schema;
using Capstone.Service.LoggerService;
using Capstone.Service.PermissionSchemaService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Capstone.API.Controllers
{
	[Route("api/schema-management")]
    [ApiController]
    public class PermissionSchemaController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IPermissionSchemaService _permissionSchemaService;

        public PermissionSchemaController(ILoggerManager logger, IPermissionSchemaService permissionSchemaService)
        {
            _logger = logger;
            _permissionSchemaService = permissionSchemaService;
        }

		[EnableQuery]
        [HttpGet("schemas")]
        public async Task<ActionResult<List<GetSchemaResponse>>> GetAllSchema()
        {
            var result = await _permissionSchemaService.GetAllSchema(true);

            return Ok(result);
        }
        
        [EnableQuery]
        [HttpGet("schemas/project-schema/{projectId:Guid}")]
        public async Task<ActionResult<List<GetSchemaResponse>>> GetProjectSchemas(Guid projectId)
        {
            var result = await _permissionSchemaService.GetProjectSchemas(projectId);

            return Ok(result);
        }
        
        [EnableQuery]
        [HttpGet("schemas/deleted")]
        public async Task<ActionResult<List<GetSchemaResponse>>> GetAllSchemaDeleted()
        {
            var result = await _permissionSchemaService.GetAllSchema(false);

            return Ok(result);
        }

        [HttpGet("schemas/{schemaId:Guid}")]
        public async Task<IActionResult> GetSchemaById(Guid schemaId)
        {
            var result = await _permissionSchemaService.GetPermissionSchemaById(schemaId);
            if(result == null)
            {
                return BadRequest("Schema not existed!");
            }
            return Ok(result);
        }

        [HttpPost("schemas")]
        public async Task<IActionResult> CreateSchema(CreateNewSchemaRequest request)
        {
            
            var role = await _permissionSchemaService.GetSchemaByName(request.SchemaName);
            if (role != null)
            {
                return BadRequest("Schema name existed!");
            }
            var result = await _permissionSchemaService.CreateNewPermissionSchema(request);
            return Ok(result);
        }

        //1
        [HttpPut("schemas")]
        public async Task<IActionResult> UpdateSchema(UpdateSchemaRequest request)
        {
			var isExist = await _permissionSchemaService.CheckExist(request.SchemaId);
			if (!isExist)
			{
				return NotFound("Schema not exist!!!");
			}
			var role = await _permissionSchemaService.GetSchemaByName(request.SchemaName);
            if (role != null && role.SchemaId != request.SchemaId)
            {
                return BadRequest("Schema name existed!");
            }
            var result = await _permissionSchemaService.UpdateSchema(request.SchemaId, request);
            if(result == true)
            {
                var schema = await _permissionSchemaService.GetSchemaById(request.SchemaId);
                return Ok(schema);
            }
            else
            {
                return StatusCode(500);
            }
        }
        //2
        [HttpPut("schemas/grant-permission")]
        public async Task<IActionResult> GrantSchemaPermissionRoles(GrantPermissionSchemaRequest request)
        {
			var isExist = await _permissionSchemaService.CheckExist(request.SchemaId);
			if (!isExist)
			{
				return NotFound("Interation not exist!!!");
			}
			var result = await _permissionSchemaService.GrantSchemaPermissionRoles(request.SchemaId, request);
            if(result == true)
            {
                var schemaDetails = await _permissionSchemaService.GetPermissionSchemaById(request.SchemaId);
                return Ok(schemaDetails);
            }
            else
            {
                return BadRequest("Can not grant this role!");
            }
        }
        //3
        [HttpPut("schemas/revoke-permission")]
        public async Task<IActionResult> RevokeSchemaPermissionRoles(RevokePermissionSchemaRequest request)
        {
			var isExist = await _permissionSchemaService.CheckExist(request.SchemaId);
			if (!isExist)
			{
				return NotFound("Interation not exist!!!");
			}
			var result = await _permissionSchemaService.RevokeSchemaPermissionRoles(request.SchemaId, request);
            if(result == true)
            {
                var schemaDetails = await _permissionSchemaService.GetPermissionSchemaById(request.SchemaId);
                return Ok(schemaDetails);
            }
            else
            {
                return BadRequest("Schema not existed!");
            }
        }

        [HttpDelete("system/schema/{schemaId}")]
        public async Task<ActionResult<GetRoleResponse>> RemoveRole(Guid schemaId)
        {
            var role = await _permissionSchemaService.GetSchemaById(schemaId);
            if (role == null)
            {
                return BadRequest("Schema not existed!");
            }
            var isRemoved = await _permissionSchemaService.RemoveSchemaAsync(schemaId);
            if (isRemoved == false)
            {
                return BadRequest("CAN NOT Removed schema!");
            }
            return Ok(isRemoved);
        }

    }
}
