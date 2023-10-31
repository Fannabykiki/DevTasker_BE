using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
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
        public async Task<ActionResult<IQueryable<GetAllPermissionSchemaResponse>>> GetAllSchema()
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
            var result = await _permissionSchemaService.CreateNewPermissionSchema(request);

            return Ok(result);
        }

        [HttpPut("schemas/{schemaId:Guid}")]
        public async Task<IActionResult> UpdateSchemaPermissionRoles(Guid schemaId, UpdatePermissionSchemaRequest request)
        {
            var result = await _permissionSchemaService.UpdateSchemaPermissionRoles(schemaId, request);

            return Ok(result);
        }

    }
}
