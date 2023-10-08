using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.User
{
	public class UpdateProfileRequest
	{
		public string? UserName { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public GenderEnum? Gender { get; set; } //Male = 0,  Female = 1, Other = 2
		public StatusEnum Status { get; set; } //Active = 1, Inactive = 2
	}
}
