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
        Task<UserViewModel> LoginUser(string username, string password);
        Task<UserViewModel> GetUserByEmailAsync(string email);
        Task<User> GetAllUserAsync();
        Task<PagedResponse<ViewPagedUsersResponse>> GetUsersAsync(int pageSize = 2, int pageNumber = 1, StatusEnum? status = null, string? search = null);
        Task<bool> DeleteAsync(Guid id);
        Task<CreateUserResponse> VerifyUser(string email);
        Task<CreateUserResponse> Register(CreateUserRequest createUserRequest);
        Task<RefreshToken> GenerateRefreshToken();
		Task<string> CreateToken(UserViewModel user);
        Task SendVerifyEmail(EmailRequest emailRequest);
        Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
		Task<bool> ResetPassWord(ResetPasswordRequest resetPasswordRequest);
		Task<bool> ForgotPassword(string email);
		Task<User> GetUserByIdAsync(Guid id);
		Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest, Guid id);
		Task<bool> SetRefreshToken(string? email, RefreshToken refreshToken);
		
	}
}
