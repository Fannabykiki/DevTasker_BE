﻿using Capstone.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Fullname { get; set; }
        public byte[]?  PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? VerificationToken { get; set; }
        public string? PassResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }
        public GenderEnum? Gender { get; set; } //Male = 0,  Female = 1, Other = 2
        public DateTime JoinedDate { get; set; }
        public bool IsFirstTime { get; set; } //true
        public Guid StatusId { get; set; }
        public Status Status { get; set; }
        public bool IsAdmin { get; set; } //True: Admin , False : User
        public List<Notification> Notifications { get; set; } // 1 user has many noti
        public List<ProjectMember> ProjectMember { get; set; } // 1 user is many porject_member in other project
    }
}
