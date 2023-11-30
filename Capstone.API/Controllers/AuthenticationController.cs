using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
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
		private readonly IConfiguration _config;

		public AuthenticationController(IUserService usersService, ILoggerManager logger, IHttpContextAccessor httpContextAccessor, IConfiguration config)
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
			_config = config;
		}


		[HttpGet("/users/profile/{id}")]
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

        [HttpPost("external-login/token")]
		public async Task<ActionResult<LoginResponse>> LoginExternalCallback(ExternalLoginRequest request)
        {
			GoogleProfile googleUser = new GoogleProfile();
			try
			{
                //var ClientSecret = _config["Authentication:Google:ClientSecret"];
                //var ClientID = _config["Authentication:Google:ClientId"];
                //var url = _config["Authentication:Google:CallBackUrl"];
                //var ggToken = await GoogleAuth.GetAuthAccessToken(code, ClientID, ClientSecret, url);
                //var userProfile = await GoogleAuth.GetProfileResponseAsync(ggToken.AccessToken.ToString());
                //googleUser = JsonConvert.DeserializeObject<GoogleProfile>(userProfile);
                var handler = new JwtSecurityTokenHandler();

				// Read and validate the token
                var tokenGG = handler.ReadJwtToken(request.code);

                var claimsDictionary = new Dictionary<string, string>();

                // Extract claims into a dictionary
                foreach (var claim in tokenGG.Claims)
                {
                    claimsDictionary.Add(claim.Type, claim.Value);
                }
                // Serialize the claims dictionary into JSON
                var jsonClaims = JsonConvert.SerializeObject(claimsDictionary);

                // Deserialize the JSON back into a GoogleProfile object
                googleUser = JsonConvert.DeserializeObject<GoogleProfile>(jsonClaims);
			}
			catch (Exception ex)
			{

			}

			var user = await _usersService.GetUserByEmailAsync(googleUser.Email);
			if (user == null)
			{
				var userGG = new CreateUserGGLoginRequest
                {
					Email = googleUser.Email,
					Address = googleUser.Locale,
					Avatar= googleUser.Picture
				};
				await _usersService.CreateNewUser(userGG);

                user = await _usersService.GetUserByEmailAsync(googleUser.Email);

			}
            if (user == null)
            {
                return NotFound("User not exist");
            }
			if (user.StatusId.Equals(Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB")))
			{
				return BadRequest("User is inactive");
			}
			if (user.VerifiedAt == null)
            {
                return BadRequest("User not verified!");
            }

            var token = await _usersService.CreateToken(user);

            var refreshToken = await _usersService.GenerateRefreshToken();

            await _usersService.SetRefreshToken(user.Email, refreshToken, token);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires,
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

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


		[HttpPost("token")]
		public async Task<ActionResult<LoginResponse>> LoginInternal(LoginRequest request)
		{
			var account = await _usersService.GetUserByEmailAsync(request.Email);
			if (account == null)
			{
				return NotFound("Your email not exist");
			}
			var user = await _usersService.LoginUser(request.Email, request.Password);
			if(user == null)
			{
				return NotFound("Your password not correct");

			}
			if (user.StatusId.Equals(Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB")))
			{
				return BadRequest("Your account is inactive.Please verify your account");
			}

			var token = await _usersService.CreateToken(user);

			var refreshToken = await _usersService.GenerateRefreshToken();

			await _usersService.SetRefreshToken(user.Email, refreshToken, token);

			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = refreshToken.Expires,
			};

			Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

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

		[HttpPost("verify-user")]
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
			if (user.StatusId.Equals(Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB")))
			{
				return BadRequest("User is inactive");
			}
			if (user.VerifiedAt == null)
			{
				return BadRequest("User not verified!");
			}

			await _usersService.ForgotPassword(email);

			return Ok("A verification email send to user");
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
		{
			var user = await _usersService.GetUserByEmailAsync(resetPasswordRequest.Email);
			if(user.PassResetToken != resetPasswordRequest.Token)
			{
				return BadRequest("Invalid Token");
			}
			if (user.ResetTokenExpires < DateTime.UtcNow )
			{
				return BadRequest("Token has expired");
			}
			if (user.StatusId.Equals(Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB")))
			{
				return BadRequest("User is inactive");
			}
			if (user.VerifiedAt == null)
			{
				return BadRequest("User not verified!");
			}
			await _usersService.ResetPassWord(resetPasswordRequest);

			return Ok("A verification email send to user");
		}


		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
		{
			var user = await _usersService.GetUserByEmailAsync(changePasswordRequest.Email);
			if (user == null || user.AccessToken != changePasswordRequest.Token)

			{
				return NotFound("Invalid token");
			}
			if (user.StatusId.Equals(Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB")))
			{
				return BadRequest("User is inactive");
			}
			if (user.VerifiedAt == null)
			{
				return BadRequest("User not verified!");
			}
            if (!await _usersService.VerifyPasswordHash(changePasswordRequest.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password not correct!");
            }
			if (changePasswordRequest.NewPassword.Equals(changePasswordRequest.ConfirmPassword) == false)
            {
                return BadRequest("New Password not match!");
            }

            await _usersService.ChangePassWord(changePasswordRequest);

			return Ok("Password have been change!");
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] CreateUserRequest createUserRequest)
		{
			var user = await _usersService.GetUserByEmailAsync(createUserRequest.Email);

			if (user != null)
			{
				return BadRequest("Email already exist.");
			}
			var result = await _usersService.Register(createUserRequest);
			var mail = await _usersService.SendVerifyEmail(createUserRequest.Email);
			if (mail == false)
			{
				return BadRequest("User not exist in system");
			}
			return Ok(result);
		}

		[HttpPost("send-email-forgot")]
		public async Task<IActionResult> SendEmailForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
		{
			await _usersService.SendResetPasswordEmail(forgotPasswordRequest);

			return Ok();
		}

		[HttpPost("refresh-token")]
		public async Task<ActionResult<string>> RefreshToken(string email)
		{
			var getRefreshToken = Request.Cookies["refreshToken"];

			var user = await _usersService.GetUserByEmailAsync(email);

			if (user.StatusId.Equals(Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB")))
			{
				return BadRequest("User is inactive");
			}
			if (user.VerifiedAt == null)
			{
				return BadRequest("User not verified!");
			}

			if (user==null) return NotFound();

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

			await _usersService.SetRefreshToken(user.Email, refreshToken, token);

			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = refreshToken.Expires,
			};

			Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

			return Ok(token);
		}
	}
}