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
        public Task<List<NotificationViewModel>> GetLatestNotifications(Guid userId, int page = 10);
        public Task<List<NotificationViewModel>> GetAllNotificationsByUser(Guid userid);
        public System.Threading.Tasks.Task SendNotificationChangeProjectStatus(Guid projectId, Guid userId);
        public System.Threading.Tasks.Task SendNotificationChangeTaskStatus(Guid taskId, Guid userId);
        public System.Threading.Tasks.Task SendNotificationDeleteTaskNotification(Guid taskId, Guid userId);
        public System.Threading.Tasks.Task SendNotificationCommentTask(Guid commentId, Guid userId, string action);
        public System.Threading.Tasks.Task SendNotificationTaskDeadline();
        public System.Threading.Tasks.Task<bool> MarkReadNotification(Guid userId, ReadNotificationRequest request);
        public System.Threading.Tasks.Task RemoveMemberNotification(Guid userId, Guid projectId, Guid removedMemberUserId);
        public System.Threading.Tasks.Task ExitProjectNotification(Guid userId, Guid projectId);
    }
}
