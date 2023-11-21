using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Notification
{
    public class NotificationViewModel
    {
        public Guid NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }
        public string? TargetUrl { get; set; }
        public bool IsRead { get; set; } //false : Not yet , True : Already
        public Guid RecerverId { get; set; }
    }
}
