﻿using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.User
{
	public class GetUserProfileResponse
	{
		public string? Fullname { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public bool? IsFirstTime { get; set; }
		public DateTime? DoB{ get; set; }
		public GenderEnum? Gender { get; set; } //Male = 0,  Female = 1, Other = 2
		public bool IsAdmin { get; set; }
        public Guid StatusId { get; set; } // Active = 1,Inactive = 2

    }
}
