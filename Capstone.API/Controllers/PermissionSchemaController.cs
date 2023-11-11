using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.Common.DTOs.Schema;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.PermissionSchemaService;
using Capstone.Service.ProjectService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

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
            var result = await _permissionSchemaService.GetAllSchema();

            return Ok(result);
        }

        [HttpGet("schemas/{schemaId:Guid}")]
        public async Task<IActionResult> GetSchemaById(Guid schemaId)
        {
            var result = await _permissionSchemaService.GetPermissionSchemaById(schemaId);

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
        
        [HttpPut("schemas/{schemaId:Guid}")]
        public async Task<IActionResult> UpdateSchema(Guid schemaId, UpdateSchemaRequest request)
        {
            var role = await _permissionSchemaService.GetSchemaByName(request.SchemaName);
            if (role != null)
            {
                return BadRequest("Schema name existed!");
            }
            var result = await _permissionSchemaService.UpdateSchema(schemaId, request);
            if(result == true)
            {
                var schema = await _permissionSchemaService.GetSchemaById(schemaId);
                return Ok(schema);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpPut("schemas/grant-permission/{schemaId:Guid}")]
        public async Task<IActionResult> GrantSchemaPermissionRoles(Guid schemaId, GrantPermissionSchemaRequest request)
        {
            var result = await _permissionSchemaService.GrantSchemaPermissionRoles(schemaId, request);
            if(result == true)
            {
                var schemaDetails = await _permissionSchemaService.GetPermissionSchemaById(schemaId);
                return Ok(schemaDetails);
            }
            else
            {
                return StatusCode(500);
            }
        }
        
        [HttpPut("schemas/revoke-permission/{schemaId:Guid}")]
        public async Task<IActionResult> RevokeSchemaPermissionRoles(Guid schemaId, RevokePermissionSchemaRequest request)
        {
            var result = await _permissionSchemaService.RevokeSchemaPermissionRoles(schemaId, request);
            if(result == true)
            {
                var schemaDetails = await _permissionSchemaService.GetPermissionSchemaById(schemaId);
                return Ok(schemaDetails);
            }
            else
            {
                return StatusCode(500);
            }
        }

    }
}
