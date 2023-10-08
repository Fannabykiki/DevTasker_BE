using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.User;
using Capstone.Common.Jwt;
using Capstone.Common.Token;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using MailKit.Security;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MimeKit.Text;

namespace Capstone.Service.UserService
{
	public class UserService : IUserService
	{
		private readonly CapstoneContext _context;
		private readonly IUserRepository _userRepository;

		public UserService(CapstoneContext context, IUserRepository userRepository)
		{
			_context = context;
			_userRepository = userRepository;
		}

		public async Task<CreateUserResponse> Register(CreateUserRequest createUserRequest)
		{
			try
			{
				var user = await _userRepository.GetAsync(user => user.Email == createUserRequest.Email, null);
				if (user == null)
				{
					CreatePasswordHash(createUserRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

					var newUserRequest = new User
					{
						UserId = Guid.NewGuid(),
						PasswordHash = passwordHash,
						PasswordSalt = passwordSalt,
						IsAdmin = false,
						JoinedDate = DateTime.UtcNow,
						IsFirstTime = true,
						VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
						Email = createUserRequest.Email,
						Status = Common.Enums.StatusEnum.Inactive,
					};

					var newUser = await _userRepository.CreateAsync(newUserRequest);
					_userRepository.SaveChanges();


					return new CreateUserResponse
					{
						IsSucced = true,
					};
				}
				else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return new CreateUserResponse
				{
					IsSucced = false,
				};
			}
		}

		public Task<bool> DeleteAsync(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<User>> GetAllUserAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			var user = await _userRepository.GetAsync(user => user.Email == email, null);

			if (user == null || email == null) return null;

			return new User
			{
				UserId = user.UserId,
				Email = user.Email,
				Gender = user.Gender,
				Avatar = user.Avatar,
				JoinedDate = user.JoinedDate,
				Status = user.Status,
				IsAdmin = user.IsAdmin,
				UserName = user.UserName,
				VerificationToken = user.VerificationToken,
				VerifiedAt = user.VerifiedAt,
				Address = user.Address,
				Attachments = user.Attachments,
				IsFirstTime = user.IsFirstTime,
				Notifications = user.Notifications,
				PassResetToken = user.PassResetToken,
				PasswordHash = user.PasswordHash,
				PasswordSalt = user.PasswordSalt,
				PhoneNumber = user.PhoneNumber,
				RefreshToken = user.RefreshToken,
				TokenCreated = user.TokenCreated,
				TokenExpires = user.TokenExpires,
				ResetTokenExpires = user.ResetTokenExpires,
			};
		}

		public async Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest, Guid id)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var user = await _userRepository.GetAsync(x => x.UserId == id, null);

					if (user == null)
						return new UpdateProfileResponse
						{
							IsSucced = false,
						};

					// Update user properties from request

					user.UserName = updateProfileRequest.UserName == null ? user.UserName : updateProfileRequest.UserName;
					user.PhoneNumber = updateProfileRequest.PhoneNumber == null ? user.PhoneNumber : updateProfileRequest.PhoneNumber;
					user.Address = updateProfileRequest.Address == null ? user.Address : updateProfileRequest.Address;
					user.Gender = updateProfileRequest.Gender == null ? user.Gender : updateProfileRequest.Gender;
					user.Status = updateProfileRequest.Status == null ? user.Status : updateProfileRequest.Status;

					// Save changes

					var result = await _userRepository.UpdateAsync(user);
					_userRepository.SaveChanges();

					transaction.Commit();
					return new UpdateProfileResponse
					{
						IsSucced = true,
					};
				}
				catch (Exception)
				{
					transaction.RollBack();

					return new UpdateProfileResponse
					{
						IsSucced = false,
					};
				}
			}
		}

		public async Task<User> GetUserByIdAsync(Guid id)
		{
			var user = await _userRepository.GetAsync(x => x.UserId == id, null);
			if (user != null)
			{
				return user;
			}
			return null;
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}
		public async Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		public async Task<CreateUserResponse> UpdateUserTokenAsync(RefreshToken updateUserRequest, string email)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var updateRequest = await _userRepository.GetAsync(s => s.Email == email, null);
					if (updateRequest == null)
					{
						return new CreateUserResponse
						{
							IsSucced = false,
						};
					}

					updateRequest.TokenCreated = updateUserRequest.Created;
					updateRequest.TokenExpires = updateUserRequest.Expires;
					updateRequest.RefreshToken = updateUserRequest.Token;

					await _userRepository.UpdateAsync(updateRequest);
					_userRepository.SaveChanges();

					transaction.Commit();

					return new CreateUserResponse
					{
						IsSucced = true,
					};
				}
				catch (Exception)
				{
					transaction.RollBack();

					return new CreateUserResponse
					{
						IsSucced = false,
					};
				}
			}
		}

		public async Task<User> LoginUser(string email, string password)
		{
			var user = await _userRepository.GetAsync(x => x.Email == email, null);
			if (user != null)
			{
				if (!await VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
				{
					return null;
				}
				else
				{
					return user;
				}
			}
			return null;
		}

		public async Task<RefreshToken> GenerateRefreshToken()
		{
			var refreshToken = new RefreshToken()
			{
				Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
				Expires = DateTime.UtcNow.AddDays(7),
				Created = DateTime.UtcNow
			};
			return refreshToken;
		}

		public async Task<string> CreateToken(User user)
		{
			var claims = new Claim[]
		   {
				new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
				new Claim("IsAdmin",user.IsAdmin.ToString()),
				new Claim("UserId",user.UserId.ToString()),
		   };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConstant.Key));

			var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var expired = DateTime.UtcNow.AddMinutes(JwtConstant.ExpiredTime);

			var token = new JwtSecurityToken(JwtConstant.Issuer,
				JwtConstant.Audience, claims,
				expires: expired, signingCredentials: signIn);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return tokenString;
		}

		public Task<CreateUserResponse> UpdateUserAsync(DateTime verifyAt)
		{
			throw new NotImplementedException();
		}

		public async Task<CreateUserResponse> VerifyUser(string email)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var updateRequest = await _userRepository.GetAsync(s => s.Email == email, null);
					if (updateRequest == null)
					{
						return new CreateUserResponse
						{
							IsSucced = false,
						};
					}

					updateRequest.VerifiedAt = DateTime.UtcNow;

					await _userRepository.UpdateAsync(updateRequest);
					_userRepository.SaveChanges();

					transaction.Commit();

					return new CreateUserResponse
					{
						IsSucced = true,
					};
				}
				catch (Exception)
				{
					transaction.RollBack();

					return new CreateUserResponse
					{
						IsSucced = false,
					};
				}
			}
		}

		public async Task SendVerifyEmail(EmailRequest emailRequest)
		{
			string verificationLink = "https://localhost:7266/api/authentication/verify-token?email=" +emailRequest.To+"&verifyToken="+emailRequest.VerifyToken;
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("namhocgioi2k1@gmail.com"));
			email.To.Add(MailboxAddress.Parse("" + emailRequest.To));
			email.Subject = "DevTakser verification step";
			email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>Please verify your email address</h1><p>Click the link below to verify your email:</p><a href=\"{verificationLink}\">Verify Email</a>" };

			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				client.Authenticate("namhocgioi2k1@gmail.com", "yectcfhxybsdezee");
				client.Send(email);
				client.Disconnect(true);
			}
		}

		public async Task<bool> ForgotPassword(string email)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var updateRequest = await _userRepository.GetAsync(s => s.Email == email, null);
					if (updateRequest == null)
					{
						return false;
					}

					updateRequest.RefreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
					updateRequest.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

					await _userRepository.UpdateAsync(updateRequest);
					_userRepository.SaveChanges();

					transaction.Commit();

					return true;
				}
				catch (Exception)
				{
					transaction.RollBack();

					return false;
				}
			}
		}
	
		public async Task<bool> ResetPassWord(ResetPasswordRequest resetPasswordRequest)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var updateRequest = await _userRepository.GetAsync(s => s.Email == resetPasswordRequest.Email, null);
					if (updateRequest == null)
					{
						return false;
					}

					CreatePasswordHash(resetPasswordRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

					updateRequest.PasswordHash = passwordHash;
					updateRequest.PasswordSalt = passwordSalt;
					updateRequest.PassResetToken = null;
					updateRequest.ResetTokenExpires = null;

					await _userRepository.UpdateAsync(updateRequest);
					_userRepository.SaveChanges();

					transaction.Commit();

					return true;
				}
				catch (Exception)
				{
					transaction.RollBack();

					return false;
				}
			}
		}
	}
}
