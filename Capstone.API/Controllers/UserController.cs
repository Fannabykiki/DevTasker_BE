using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPut("/users/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateProfileRequest request)
        {
            // Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _usersService.UpdateProfileAsync(request, id);

            if (result == null)
            return BadRequest(result);

            return Ok(result);

        }
    }
}
