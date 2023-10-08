using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.User;
using Capstone.Common.Token;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.API.Controllers
{
	[Route("api/authentication")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IUserService _usersService;
		private readonly ClaimsIdentity? _identity;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthenticationController(IUserService usersService, ILoggerManager logger, IHttpContextAccessor httpContextAccessor)
		{
			_usersService = usersService;
			_logger = logger;
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
		}

		[HttpPost("token")]
		public async Task<ActionResult<LoginResponse>> LoginInternal(LoginRequest request)
		{
			var user = await _usersService.LoginUser(request.Email, request.Password);
			if (user == null )
			{
				return NotFound("User not exist");
			}
			if(user.VerifiedAt == null)
			{
				return BadRequest("User not verified!");
			}

			var token = await _usersService.CreateToken(user);

			var refreshToken = await _usersService.GenerateRefreshToken();

			SetRefreshToken(user.Email, refreshToken);

			return new LoginResponse
			{
				IsAdmin = user.IsAdmin,
				UserId = user.UserId,
				Email = user.Email,
				Token = token,
				IsFirstTime = user.IsFirstTime,
				IsVerify = user.VerifiedAt,
				VerifyToken = user.VerificationToken
			};
		}

		[HttpPost("verify-token")]
		public async Task<IActionResult> VerifyEmail(string email,string verifyToken)
		{
			var user = await _usersService.GetUserByEmailAsync(email);
			if(user.VerificationToken != verifyToken)
			{
				return BadRequest("Invalid token");
			}

			await _usersService.VerifyUser(email);

			return Ok("User verified!");
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			var user = await _usersService.GetUserByEmailAsync(email);

			if (user == null)
			{
				return NotFound("User not exist");
			}

			await _usersService.ForgotPassword(email);

			return Ok("A verification email send to user");
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
		{
			var user = await _usersService.GetUserByEmailAsync(resetPasswordRequest.Email);
			if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
			{
				return NotFound("Invalid token");
			}

			await _usersService.ResetPassWord(resetPasswordRequest);

			return Ok("A verification email send to user");
		}


		[HttpPost("verify-email")]
		public async Task<IActionResult> SendEmail(EmailRequest emailRequest)
		{
			await _usersService.SendVerifyEmail(emailRequest);

			return Ok();
		}
		private async Task<IActionResult> SetRefreshToken(string email, RefreshToken refreshToken)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = refreshToken.Expires,
			};

			Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

			var result = await _usersService.UpdateUserTokenAsync(refreshToken, email);

			if (result == null) return StatusCode(500);

			return Ok(result);
		}

		[HttpPost("refresh-token")]
		public async Task<ActionResult<string>> RefreshToken(string email)
		{
			var getRefreshToken = Request.Cookies["refreshToken"];

			var user = await _usersService.GetUserByEmailAsync(email);

			if(user==null) return NotFound();

			if (!user.RefreshToken.Equals(getRefreshToken))
			{
				return Unauthorized("Invalid Refresh Token.");
			}

			else if (user.TokenExpires < DateTime.Now)
			{
				return Unauthorized("Token expired.");
			}

			var token = await _usersService.CreateToken(user);

			var refreshToken = await _usersService.GenerateRefreshToken();

			SetRefreshToken(email, refreshToken);

			return Ok(token);
		}

	}
}
