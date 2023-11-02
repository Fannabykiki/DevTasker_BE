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
using System.Security.Cryptography;
using System.Text;
using MimeKit.Text;
using AutoMapper;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Capstone.Service.UserService
{
	public class UserService : IUserService
	{
		private readonly CapstoneContext _context;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public UserService(CapstoneContext context, IUserRepository userRepository, IMapper mapper, IServiceScopeFactory serviceScopeFactory)
		{
			_context = context;
			_userRepository = userRepository;
			_mapper = mapper;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public async Task<CreateUserResponse> Register(CreateUserRequest createUserRequest)
		{
			try
			{
				var user = await _userRepository.GetAsync(user => user.Email == createUserRequest.Email, null)!;
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
						StatusId = Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB"),
					};

					var newUser = await _userRepository.CreateAsync(newUserRequest);
					await _userRepository.SaveChanges();

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

        public async Task<CreateUserResponse?> CreateNewUser(CreateUserGGLoginRequest createUserGGLoginRequest)
        {
            try
            {
                var user = await _userRepository.GetAsync(user => user.Email == createUserGGLoginRequest.Email, null)!;
                if (user == null)
				{
					Random random = new Random();
                    CreatePasswordHash(Convert.ToString(RandomNumberGenerator.GetBytes(64)), out byte[] passwordHash, out byte[] passwordSalt);

                    var newUserRequest = new User
                    {
                        UserId = Guid.NewGuid(),
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        IsAdmin = false,
                        JoinedDate = DateTime.UtcNow,
                        IsFirstTime = true,
                        VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
                        Email = createUserGGLoginRequest.Email,
                        StatusId = Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DFFB"),
						VerifiedAt = DateTime.UtcNow
                    };

                    var newUser = await _userRepository.CreateAsync(newUserRequest);
                    await _userRepository.SaveChanges();

                    return new CreateUserResponse
                    {
                        IsSucced = true,
                        VerifyToken = newUser.VerificationToken
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

		public async Task<UserViewModel> GetUserByEmailAsync(string email)
		{
			var user = await _userRepository.GetAsync(user => user.Email == email, null)!;

			if (user == null || email == null) return null;

			return _mapper.Map<UserViewModel>(user);
		}

		public async Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest, Guid id)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var user = await _userRepository.GetAsync(x => x.UserId == id, null)!;

					if (user == null)
						return new UpdateProfileResponse
						{
							IsSucced = false,
						};

					// Update user properties from request

                    user.Fullname = updateProfileRequest.Fullname ?? user.Fullname;
                    user.UserName = updateProfileRequest.UserName ?? user.UserName;
                    user.PhoneNumber = updateProfileRequest.PhoneNumber ?? user.PhoneNumber;
                    user.Address = updateProfileRequest.Address ?? user.Address;
                    user.Dob = updateProfileRequest.DoB ?? user.Dob;
                    user.Gender = updateProfileRequest.Gender ?? user.Gender;
					user.IsFirstTime = false;
                    // Save changes

					var result = await _userRepository.UpdateAsync(user);
					await _userRepository.SaveChanges();

					transaction.Commit();
					return new UpdateProfileResponse
					{
						IsSucced = true,
						VerifyToken = result.VerificationToken
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
			var user = await _userRepository.GetAsync(x => x.UserId == id, null)!;
			return user;
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}
		public Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using var hmac = new HMACSHA512(passwordSalt);
			var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			return Task.FromResult(computedHash.SequenceEqual(passwordHash));
		}

		public async Task<UserViewModel> LoginUser(string email, string password)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<CapstoneContext>();
			using (var transaction = context.Database.BeginTransaction())
			{
				try
				{
					var user = await _userRepository.GetAsync(x => x.Email == email, null)!;

					if (user.PasswordSalt != null && user.PasswordHash != null && !await VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
					{
						return null;
					}
					return _mapper.Map<UserViewModel>(user);
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					throw ex;
				}
				finally
				{
					transaction.Dispose();
				}

			}
		}

		public Task<RefreshToken> GenerateRefreshToken()
		{
			var refreshToken = new RefreshToken()
			{
				Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
				Expires = DateTime.UtcNow.AddDays(7),
				Created = DateTime.UtcNow
			};
			return Task.FromResult(refreshToken);
		}

		public Task<string> CreateToken(UserViewModel user)
		{
			var claims = new Claim[]
		  {
			new Claim("UserId",user.UserId.ToString()),
		  };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConstant.Key));

			var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var expired = DateTime.UtcNow.AddMinutes(JwtConstant.ExpiredTime);

			var token = new JwtSecurityToken(JwtConstant.Issuer,
				JwtConstant.Audience, claims,
				expires: expired, signingCredentials: signIn);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return Task.FromResult(tokenString);
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
					var updateRequest = await _userRepository.GetAsync(s => s.Email == email, null)!;

					updateRequest.StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE");
					updateRequest.VerifiedAt = DateTime.UtcNow;

					await _userRepository.UpdateAsync(updateRequest);
					await _userRepository.SaveChanges();

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

		public async Task<bool> SendVerifyEmail(string emailRequest)
		{
			var updateRequest = await _userRepository.GetAsync(s => s.Email == emailRequest, null)!;
			string verificationLink = "https://devtasker.azurewebsites.net/verify-account?"+ "token=" + updateRequest.VerificationToken +"&email=" + emailRequest;
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("devtaskercapstone@gmail.com"));
			email.To.Add(MailboxAddress.Parse("" + emailRequest));
			email.Subject = "DevTakser verification step";
			email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>Please verify your email address</h1><p>Click the link below to verify your email:</p><a href=\"{verificationLink}\">Verify Email</a>" };

			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				client.Authenticate("devtaskercapstone@gmail.com", "fbacmmlfxlmchkmc");
				client.Send(email);
				client.Disconnect(true);
			}

			return true;
		}

		public async Task<bool> ForgotPassword(string email)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var updateRequest = await _userRepository.GetAsync(s => s.Email == email, null)!;

					updateRequest.PassResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
					updateRequest.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

					await _userRepository.UpdateAsync(updateRequest);
					await _userRepository.SaveChanges();

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
					var updateRequest = await _userRepository.GetAsync(s => s.Email == resetPasswordRequest.Email, null)!;

					CreatePasswordHash(resetPasswordRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

					updateRequest.PasswordHash = passwordHash;
					updateRequest.PasswordSalt = passwordSalt;
					updateRequest.PassResetToken = null;
					updateRequest.ResetTokenExpires = null;

					await _userRepository.UpdateAsync(updateRequest);
					await _userRepository.SaveChanges();

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
		public async Task<bool> ChangePassWord(ChangePasswordRequest changePasswordRequest)
		{
			using (var transaction = _userRepository.DatabaseTransaction())
			{
				try
				{
					var updateRequest = await _userRepository.GetAsync(s => s.Email == changePasswordRequest.Email, null)!;

					CreatePasswordHash(changePasswordRequest.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

					updateRequest.PasswordHash = passwordHash;
					updateRequest.PasswordSalt = passwordSalt;
					updateRequest.PassResetToken = null;
					updateRequest.ResetTokenExpires = null;

                    await _userRepository.UpdateAsync(updateRequest);
                    await _userRepository.SaveChanges();
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
        public async Task<bool> ChangeUserStatus(ChangeUserStatusRequest changeUserStatusRequest, Guid userId)
        {
            using (var transaction = _userRepository.DatabaseTransaction())
            {
                try
                {
                    var updateRequest = await _userRepository.GetAsync(s => s.UserId == userId, null)!;

                    updateRequest.StatusId = changeUserStatusRequest.StatusIdChangeTo;

                    await _userRepository.UpdateAsync(updateRequest);
                    await _userRepository.SaveChanges();

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

		public async Task<bool> SetRefreshToken(string? email, RefreshToken refreshToken, string accessToken)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<CapstoneContext>();
			using (var transaction = context.Database.BeginTransaction())
			{
				try
				{	
					var updateRequest = await _userRepository.GetAsync(s => s.Email == email, null)!;

					updateRequest.AccessToken = accessToken;
					updateRequest.TokenCreated = refreshToken.Created;
					updateRequest.TokenExpires = refreshToken.Expires;
					updateRequest.RefreshToken = refreshToken.Token;

					await _userRepository.UpdateAsync(updateRequest);
					await _userRepository.SaveChanges();

					transaction.Commit();

					return true;
				}
				catch (Exception)
				{
					transaction.Rollback();

					return false;
				}
				finally
				{
					transaction.Dispose();
				}
			}
		}

		public async Task<List<ViewPagedUsersResponse>> GetUsersAsync()
		{
			IQueryable<User> query = _userRepository.GetAllAsync(x => true, null);
			var users = query.Select(x => new ViewPagedUsersResponse
			{
				Id = x.UserId,
				Email = x.Email,
				Name = x.UserName,
				StatusId = x.StatusId, //Get status name instead of id
			}).ToList();
			return users;
			
		}

		public async Task<bool> SendResetPasswordEmail(ForgotPasswordRequest forgotPasswordRequest)
		{
			var forgotRequest = await _userRepository.GetAsync(s => s.Email == forgotPasswordRequest.To, null)!;

			string verificationLink = "https://devtasker.azurewebsites.net/create-newpwd?" + forgotRequest.PassResetToken;
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("devtaskercapstone@gmail.com"));
			email.To.Add(MailboxAddress.Parse("" + forgotPasswordRequest.To));
			email.Subject = "DevTakser reset password";
			email.Body = new TextPart(TextFormat.Html) { Text = $"<p>Click the link below to verify your email:</p><a href=\"{verificationLink}\">Verify Email</a>" };

			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				client.Authenticate("devtaskercapstone@gmail.com", "fbacmmlfxlmchkmc");
				client.Send(email);
				client.Disconnect(true);
			}
			return true;
		}
	}
}
