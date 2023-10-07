using Capstone.Common.DTOs.User;
using Capstone.Common.Token;
using Capstone.DataAccess.Entities;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Capstone.API.Controllers
{
	[Route("api/authentication")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
		private readonly IUserService _usersService;
		private readonly ClaimsIdentity? _identity;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthenticationController(IUserService usersService, ILoggerManager logger, IHttpContextAccessor httpContextAccessor, IConfiguration config)
		{
            _config = config;
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
			var user = await _usersService.LoginUser(request.UserName, request.Password);
			if (user == null)
			{
				return NotFound();
			}

			var token = await _usersService.CreateToken(user);

			var refreshToken = await _usersService.GenerateRefreshToken();

			SetRefreshToken(user.Email, refreshToken);

			return new LoginResponse
			{
				IsAdmin = user.IsAdmin,
				UserId = user.UserId,
				UserName = user.UserName,
				Email = user.Email,
				Token = token,
				IsFirstTime = user.IsFirstTime
			};
		}

        [HttpGet("external-login")]
        public IActionResult Login()
        {

            var clientId = _config["Authentication:Google:ClientId"];

            var redirectUrl = _config["Authentication:Google:CallBackUrl"];

            var authUrl = GoogleAuth.GetAuthUrl(clientId, redirectUrl);

            return Redirect(authUrl);

        }

        [HttpGet("external-login/token")]
        public async Task<ActionResult<LoginResponse>> LoginExternalCallback(string? code)
        {
            GoogleProfile googleUser = new GoogleProfile();
            try
            {
                var ClientSecret = _config["Authentication:Google:ClientSecret"];
                var ClientID = _config["Authentication:Google:ClientId"];
                var url = _config["Authentication:Google:CallBackUrl"];
                var ggToken = await GoogleAuth.GetAuthAccessToken(code, ClientID, ClientSecret, url);
                var userProfile = await GoogleAuth.GetProfileResponseAsync(ggToken.AccessToken.ToString());
                googleUser = JsonConvert.DeserializeObject<GoogleProfile>(userProfile);

            }
            catch (Exception ex)
            {

            }

            var user = await _usersService.GetUserByEmailAsync(googleUser.Email);
            if (user == null)
            {
                return NotFound();
            }
            var token = await _usersService.CreateToken(user);

            var refreshToken = await _usersService.GenerateRefreshToken();

            SetRefreshToken(user.Email, refreshToken);

            return new LoginResponse
            {
                IsAdmin = user.IsAdmin,
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Token = token,
                IsFirstTime = user.IsFirstTime
            };
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
