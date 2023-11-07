using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.User
{
    public class GetUsersAnalyzeResponse
    {
        public int TotalUser { get; set; }
        public int ActiveUsers { get; set; }
        public int PercentActive { get; set; }
        public int InactiveUser { get; set; }
        public int PercentInactive { get; set; }
    }
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? StatusName { get; set; }
        public bool IsAdmin { get; set; }
        public string? Address { get; set; }
        public DateTime? Dob { get; set; }
        //True: Admin , False : User
    }
}
