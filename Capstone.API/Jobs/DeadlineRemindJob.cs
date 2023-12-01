using Capstone.Service.NotificationService;

namespace Capstone.API.Jobs
{
    public class DeadlineRemindJob:IEmailJob
    {
        private readonly INotificationService _notificationService;
        public DeadlineRemindJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task RunJob()
        {
            try
            {
                await _notificationService.SendNotificationTaskDeadline();
            }
            catch (Exception ex)
            {
                ;
            }
        }
    }
}
