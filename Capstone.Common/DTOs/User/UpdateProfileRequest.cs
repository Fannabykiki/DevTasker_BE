using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.User
{
    public class UpdateProfileRequest
    {
        public string? UserName { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public GenderEnum Gender { get; set; } //Male = 0,  Female = 1, Other = 2
        public bool IsFirstTime { get; set; } //true
    }
}
