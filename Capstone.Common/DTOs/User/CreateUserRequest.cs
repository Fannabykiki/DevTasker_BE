using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.User
{
    public class CreateUserRequest
    {
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }
    }
}
