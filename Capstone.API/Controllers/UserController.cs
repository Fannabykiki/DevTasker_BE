using Capstone.Common.DTOs.Paging;
using Capstone.Common.DTOs.User;
using Capstone.Common.Enums;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone.API.Controllers
{
    [Route("api/user-management")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUserService _usersService;

        public UserController(ILoggerManager logger, IUserService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        [HttpPost("users-paged")]
        public async Task<ActionResult<PagedResponse<ViewPagedUsersResponse>>> GetUsers(PagedRequest pagedRequest)
        {
            var response = await _usersService.GetUsersAsync(pagedRequest.pageSize, pagedRequest.pageNumber, pagedRequest.status, pagedRequest.search);
            return Ok(response);

        }

        [HttpPost("users")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest createUserRequest)
        { 
            var user = await _usersService.GetUserByEmailAsync(createUserRequest.Email);

            if(user != null)
            {
                return BadRequest("Email already exist.");
            }

            var result = await _usersService.Register(createUserRequest);

            if (result == null) return StatusCode(500);

            return Ok(result);
        }
    }
}
