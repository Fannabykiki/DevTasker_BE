using Capstone.Common.DTOs.Email;
using Capstone.API.Helper;
using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.StatusService;
using Capstone.Service.UserService;
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
		private readonly IMailHelper _mailHelper;
		private readonly IUserService _usersService;
		private readonly IStatusService _statusService;
		private readonly ClaimsIdentity? _identity;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _config;


		public UserController(ILoggerManager logger, IUserService usersService, IConfiguration config, IHttpContextAccessor httpContextAccessor, IMailHelper mailHelper, IStatusService statusService)
		{
			_logger = logger;
            _mailHelper = mailHelper;
            _usersService = usersService;
			_statusService = statusService;
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

		[HttpGet("admin/users")]
		[EnableQuery()]
		public async Task<ActionResult<List<UserResponse>>> GetUsers()
		{
			var response = await _usersService.GetUsersAsync();
			if (response == null)
			{
				return BadRequest("Three are no User!");
			}
			return Ok(response);
		}
		
		[HttpGet("admin/users/analyzation")]
		public async Task<ActionResult<GetUsersAnalyzeResponse>> GetUsersAnalyze()
		{
			var response = await _usersService.GetUsersAnalyze();
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
				Fullname = user.Fullname,
				UserName = user.UserName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Address = user.Address,
				DoB = user.Dob,
				Gender = user.Gender,
				IsAdmin = user.IsAdmin,
                StatusId = user.StatusId,
				IsFirstTime = user.IsFirstTime,
            };
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
		
		[HttpPut("users/change-status/{id}")]
		public async Task<IActionResult> ChangeUserStatus(ChangeUserStatusRequest changeUserStatusRequest,Guid id)
		{
            var user = await _usersService.GetUserByIdAsync(changeUserStatusRequest.ChangeBy);
			if (user.IsAdmin == null || user.IsAdmin == false)
			{
				return Unauthorized();
			}
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow || user.AccessToken  != changeUserStatusRequest.VerifyToken)
            {
                return NotFound("Invalid token");
            }

            var result = await _usersService.ChangeUserStatus(changeUserStatusRequest, id);
			if (result == true)
			{
				var status = await _statusService.GetStatusByIdAsync(id);
                var userBeChange = await _usersService.GetUserByIdAsync(id);
				await _mailHelper.Send(userBeChange.Email, $"[DevTasker] Your account status have been change to {status.Title}", changeUserStatusRequest.reason);
            }
            

            return Ok(result);

		}
	}
}
