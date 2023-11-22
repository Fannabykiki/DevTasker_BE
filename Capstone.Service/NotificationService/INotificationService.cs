using Capstone.Common.DTOs.Notification;
using Capstone.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.NotificationService
{
    public interface INotificationService
    {
        public Task<List<NotificationViewModel>> GetLatestNotifications(Guid userid, int page = 10);
    }
}
