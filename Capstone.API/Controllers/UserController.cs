using Capstone.Common.DTOs.Paging;
using Capstone.Common.DTOs.User;
using Capstone.Common.Enums;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Security.Claims;

namespace Capstone.API.Controllers
{
    [Route("api/user-management")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUserService _usersService;
		private readonly ClaimsIdentity? _identity;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _config;


		public UserController(ILoggerManager logger, IUserService usersService, IConfiguration config, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_usersService = usersService;
			_httpContextAccessor = httpContextAccessor;
			var identity = httpContextAccessor.HttpContext?.User?.Identity;
			if (identity == null)
			{
				_identity = null;
			}
			else
			{
				_identity = identity as ClaimsIdentity;
			}
			_config = config;
        }

        [HttpGet("users")]
        [EnableQuery()]
        public async Task<ActionResult<ViewPagedUsersResponse>> GetUsers()
        {
            var response = await _usersService.GetUsersAsync();
            if (response == null)
            {
                return BadRequest("Three are no User!");
            }
            return Ok(response);

        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<GetUserProfileResponse>> GetUserProfile(Guid id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return new GetUserProfileResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Avatar = user.Avatar,
                Gender = user.Gender,
                Status = user.Status,
                IsAdmin = user.IsAdmin,
            };

        }

        [HttpPost("users")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest createUserRequest)
        {
	        if (createUserRequest.Email != null)
	        {
		        await _usersService.GetUserByEmailAsync(createUserRequest.Email);

		        return BadRequest("Email already exist.");
	        }

	        var result = await _usersService.Register(createUserRequest);

	        return Ok(result);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateProfileRequest request)
        {
            // Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _usersService.UpdateProfileAsync(request, id);

            return Ok(result);

        }
    }
}
