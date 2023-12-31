﻿using Capstone.Common.DTOs.Email;
using Capstone.Common.DTOs.Paging;
using Capstone.Common.DTOs.User;
using Capstone.Common.Token;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.UserService
{
    public interface IUserService
    {
        Task<UserViewModel> LoginUser(string username, string password);
        Task<UserViewModel> GetUserByEmailAsync(string email);
        Task<List<UserResponse>> GetUsersAsync();
        Task<GetUsersAnalyzeResponse> GetUsersAnalyze();
        Task<bool> DeleteAsync(Guid id);
        Task<CreateUserResponse> VerifyUser(string email);
        Task<CreateUserResponse> Register(CreateUserRequest createUserRequest);
        Task<CreateUserResponse> CreateNewUser(CreateUserGGLoginRequest createUserGGLoginRequest);
        Task<RefreshToken> GenerateRefreshToken();
		Task<string> CreateToken(UserViewModel user);
        Task<bool> SendVerifyEmail(string emailRequest);
        Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
		Task<bool> ResetPassWord(ResetPasswordRequest resetPasswordRequest);
		Task<bool> ChangePassWord(ChangePasswordRequest changePasswordRequest);
		Task<bool> ChangeUserStatus(ChangeUserStatusRequest changeUserStatusRequest, Guid userId);
		Task<bool> ForgotPassword(string email);
		Task<User> GetUserByIdAsync(Guid id);
		Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest, Guid id);
		Task<bool> SetRefreshToken(string? email, RefreshToken refreshToken,string accessToken);
		Task<bool> SendResetPasswordEmail(ForgotPasswordRequest forgotPasswordRequest);
	}
}
