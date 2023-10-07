using Capstone.Common.DTOs.User;
using Capstone.Common.Jwt;
using Capstone.Common.Token;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<CreateUserResponse> CreateAsync(CreateUserRequest createUserRequest)
        {
            using var transaction = _userRepository.DatabaseTransaction();
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
                        Email = createUserRequest.Email,
                        Status = Common.Enums.StatusEnum.Inactive,
                    };

                    var newUser = await _userRepository.CreateAsync(newUserRequest);
                    _userRepository.SaveChanges();

                    transaction.Commit();

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
                transaction.RollBack();

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
            };
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
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

        public async Task<User> LoginUser(string username, string password)
        {
            var user = await _userRepository.GetAsync(x => x.UserName == username, null);
            if (user != null)
            {
                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
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
				new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
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

		public Task<CreateUserResponse> UpdateUserAsync(UpdateUserRequest updateUserRequest, Guid id)
		{
			throw new NotImplementedException();
		}
	}
}
