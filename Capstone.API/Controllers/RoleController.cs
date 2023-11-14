using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.Service.LoggerService;
using Capstone.Service.RoleService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;

namespace Capstone.API.Controllers
{
    [Route("api/role-management")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILoggerManager _logger;

		public RoleController(IRoleService roleService, ILoggerManager logger)
		{
			_roleService = roleService;
			_logger = logger;
		}

		[EnableQuery]
        [HttpGet("system/roles")]
        public async Task<ActionResult<List<GetRoleRecord>>> GetRoles()
        {
            var roles = await _roleService.GetAllSystemRole(true);
            if (roles == null)
            {
                return BadRequest("Data null");
            }
            return Ok(roles);
        }
        
        [EnableQuery]
        [HttpGet("project/roles/{projectId}")]
        public async Task<ActionResult<List<GetRoleRecord>>> GetRolesByProjectId(Guid projectId)
        {
            var roles = await _roleService.GetRolesByProjectId(projectId);
            if (roles == null)
            {
                return BadRequest("Please check Permission schema of your Project!");
            }
            return Ok(roles);
        }
        
        [EnableQuery]
        [HttpGet("system/roles/deleted")]
        public async Task<ActionResult<List<GetRoleRecord>>> GetRolesDeleted()
        {
            var roles = await _roleService.GetAllSystemRole(false);
            if (roles == null)
            {
                return BadRequest("Data null");
            }
            return Ok(roles);
        }
        
        [EnableQuery]
        [HttpPost("permission/roles/grant/{schemaId}")]
        public async Task<ActionResult<List<GetRoleResponse>>> GetRoleToGrant(Guid schemaId, EditSchemaRoleRequest request)
        {
            var roles = await _roleService.GetRoleToEdit(schemaId, request, true);
            if (roles.Count() == 0)
            {
                return BadRequest("System not have role to grant!");
            }
            return Ok(roles);
        }
        
        [EnableQuery]
        [HttpPost("permission/roles/revoke/{schemaId}")]
        public async Task<ActionResult<List<GetRoleResponse>>> GetRoleToRevoke(Guid schemaId, EditSchemaRoleRequest request)
        {
            var roles = await _roleService.GetRoleToEdit(schemaId, request, false);
            if (roles.Count() == 0)
            {
                return BadRequest("You can not revoke default role!");
            }
            return Ok(roles);
        }

        [HttpPost("system/roles")]
        public async Task<ActionResult<GetRoleResponse>> CreateRole(CreateNewRoleRequest createRoleRequest)
        {
            var role = await _roleService.GetSystemRoleByName(createRoleRequest.RoleName);
            if (role != null)
            {
                return BadRequest("Role name existed!");
            }
            var result = await _roleService.CreateProjectRole(createRoleRequest);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }

        [HttpPut("system/roles/{id}")]
        public async Task<ActionResult<GetRoleResponse>> UpdateRole(Guid id, UpdateRoleRequest request)
        {
            var updatedRole = await _roleService.UpdateSystemRole(id, request);
            if (updatedRole == null)
            {
                return NotFound();
            }
            return Ok(updatedRole);
        }
        
        [HttpDelete("system/roles/{roleId}")]
        public async Task<ActionResult<GetRoleResponse>> RemoveeRole(Guid roleId)
        {
            var role = await _roleService.GetSystemRoleById(roleId);
            if (role == null)
            {
                return BadRequest("Role not existed!");
            }
            var isRemoved = await _roleService.RemoveSystemRoleAsync(roleId);
            if (isRemoved == null)
            {
                return NotFound();
            } 
            return Ok(isRemoved);
        }
    }
}
