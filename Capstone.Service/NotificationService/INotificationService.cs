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
        public System.Threading.Tasks.Task SendNotificationChangeProjectStatus(Guid projectId, Guid userId);
        public System.Threading.Tasks.Task SendNotificationChangeTaskStatus(Guid taskId, Guid userId);
        public System.Threading.Tasks.Task SendNotificationCommentTask(Guid commentId, Guid userId, string action);
        public System.Threading.Tasks.Task SendNotificationTaskDeadline();
        public System.Threading.Tasks.Task<bool> MarkReadNotification(Guid userId, ReadNotificationRequest request);
    }
}
