using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.Paging;
using Capstone.Common.DTOs.User;
using Capstone.Common.Enums;
using Capstone.Common.Token;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.UserService
{
    public interface IUserService
    {
        Task<User> LoginUser(string username, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<List<ViewPagedUsersResponse>> GetUsersAsync();
        Task<bool> DeleteAsync(Guid id);
        Task<CreateUserResponse> VerifyUser(string email);
        Task<CreateUserResponse> Register(CreateUserRequest createUserRequest);
        Task<RefreshToken> GenerateRefreshToken();
		Task<string> CreateToken(User user);
        Task SendVerifyEmail(EmailRequest emailRequest);
        Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
		Task<bool> ResetPassWord(ResetPasswordRequest resetPasswordRequest);
		Task<bool> ChangePassWord(ChangePasswordRequest changePasswordRequest);
		Task<bool> ChangeUserStatus(ChangePasswordRequest changePasswordRequest);
		Task<bool> ForgotPassword(string email);
		Task<User> GetUserByIdAsync(Guid id);
		Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest, Guid id);
		Task<bool> SetRefreshToken(string? email, RefreshToken refreshToken);
		
	}
}
