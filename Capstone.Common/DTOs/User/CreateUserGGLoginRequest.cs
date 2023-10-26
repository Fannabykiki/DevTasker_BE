using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.User
{
    public class CreateUserGGLoginRequest
    {
        public string Avatar { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }
}
