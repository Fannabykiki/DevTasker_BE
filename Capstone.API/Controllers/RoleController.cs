﻿using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.Service.LoggerService;
using Capstone.Service.RoleService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

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
        public async Task<ActionResult<GetAllRoleReponse>> GetRoles()
        {
            var roles = await _roleService.GetAllSystemRole(true);
            if (roles == null)
            {
                return BadRequest("Data null");
            }
            return Ok(roles);
        }
        
        [EnableQuery]
        [HttpGet("system/roles/deleted")]
        public async Task<ActionResult<GetAllRoleReponse>> GetRolesDeleted()
        {
            var roles = await _roleService.GetAllSystemRole(false);
            if (roles == null)
            {
                return BadRequest("Data null");
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
            var role = await _roleService.GetSystemRoleByName(request.RoleName);
            if (role != null)
            {
                return BadRequest("Role name existed!");
            }
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
