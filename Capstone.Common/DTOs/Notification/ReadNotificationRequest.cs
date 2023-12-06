using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Notification
{
    public class ReadNotificationRequest
    {
        public List<Guid?> ListNotificationIds { get; set; }
    }
}
