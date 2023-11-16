using Capstone.Common.DTOs.Base;
using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.User
{
	public class UserViewModel
	{
		public Guid UserId { get; set; }
		public string? UserName { get; set; }
		public string? Fullname { get; set; }
		public byte[]? PasswordHash { get; set; }
		public byte[]? PasswordSalt { get; set; }
		public string? Email { get; set; }
		public string? Address { get; set; }
		public DateTime? Dob { get; set; }
		public string? PhoneNumber { get; set; }
        public string? AccessToken { get; set; }
        public string? VerificationToken { get; set; }
		public string? PassResetToken { get; set; }
		public DateTime? ResetTokenExpires { get; set; }
		public DateTime? VerifiedAt { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? TokenCreated { get; set; }
		public DateTime? TokenExpires { get; set; }
		public GenderEnum? Gender { get; set; } //Male = 0,  Female = 1, Other = 2
		public DateTime JoinedDate { get; set; }
		public bool IsFirstTime { get; set; } //true
		public Guid StatusId { get; set; } // Active = 1,Inactive = 2
		public bool IsAdmin { get; set; } //True: Admin , False : User
		public BaseResponse BaseResponse { get; set; } //True: Admin , False : User
	}
}
