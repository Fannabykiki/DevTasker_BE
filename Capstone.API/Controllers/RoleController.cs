using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.RoleService;
using Microsoft.AspNetCore.Http;
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
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<GetRoleResponse>>> GetRoles()
        {
            var roles = await _roleService.GetAllSystemRole();
            if (roles == null)
            {
                return BadRequest("Data null");
            }
            return Ok(roles);
        }

        //[HttpPost("roles")]
        //public async Task<ActionResult<GetRoleResponse>> CreateRole(Common.DTOs.Role.CreateRoleRequest createRoleRequest)
        //{
        //    var role = _roleRepository.GetAsync(x => x.RoleName.Trim().ToLower().Equals(createRoleRequest.RoleName.Trim().ToLower()), null);
        //    if (role != null) {
        //        return BadRequest("Role name existed!");
        //    }
        //    var result = await _roleService.CreateProjectRole(createRoleRequest);
        //    if (result == null)
        //    {
        //        return StatusCode(500);
        //    }

        //    return Ok(result);
        //}

        //[HttpPut("roles/{id}")]
        //public async Task<ActionResult<GetRoleResponse>> UpdateRole(Guid id, UpdateRoleRequest request)
        //{
        //    var role = _roleRepository.GetAsync(x => x.RoleName.Trim().ToLower().Equals(request.RoleName.Trim().ToLower()), null);
        //    if (role != null)
        //    {
        //        return BadRequest("Role name existed!");
        //    }
        //    var updatedRole = await _roleService.UpdateSystemRole(id, request);
        //    if (updatedRole == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(updatedRole);
        //}
    }
}
