using Capstone.Common.DTOs.User;
using Capstone.Common.Token;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.UserService
{
    public interface IUserService
    {
        Task<User> LoginUser(string username, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<bool> DeleteAsync(Guid id);
        Task<CreateUserResponse> UpdateUserTokenAsync(RefreshToken updateUserRequest, string email);
        Task<CreateUserResponse> UpdateUserAsync(UpdateUserRequest updateUserRequest, Guid id);
        Task<CreateUserResponse> CreateAsync(CreateUserRequest createUserRequest);
        Task<RefreshToken> GenerateRefreshToken();
		Task<string> CreateToken(User user);
	}
}
