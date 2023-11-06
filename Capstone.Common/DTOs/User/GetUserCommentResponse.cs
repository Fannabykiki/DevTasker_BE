using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.User
{
    public class GetUserCommentResponse
    {
        public Guid UserId { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public bool? IsFirstTime { get; set; }
        public bool IsAdmin { get; set; }
        public string Status { get; set; }
    }
}
