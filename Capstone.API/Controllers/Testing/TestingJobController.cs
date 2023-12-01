using Capstone.API.Extentions;
using Capstone.API.Jobs;
using Capstone.Service.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers.Testing
{
    [Route("api/job-testing")]
    [ApiController]
    public class TestingJobController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailJob _emailJob;
        public TestingJobController(IEmailJob emailJob, INotificationService notificationService)
        {
            _emailJob = emailJob;
            _notificationService = notificationService;
        }
        [HttpGet("job-run")]
        public async Task<IActionResult> GetJobRun()
        {
            await _emailJob.RunJob();
            return Ok();
        }
        [HttpPost("send-notification-project")]
        public async Task<IActionResult> SendNotifcationProject(string projectId)
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationChangeProjectStatus(projectId, userId.ToString());
            return Ok();
        }
        [HttpPost("send-notification-task")]
        public async Task<IActionResult> SendNotifcationTask(string taskId)
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationChangeTaskStatus(taskId, userId.ToString());
            return Ok();
        }
        [HttpPost("send-notification-comment")]
        public async Task<IActionResult> SendNotifcationCommentTask(string commentTaskId, string action)
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationCommentTask(commentTaskId, userId.ToString(), action);
            return Ok();
        }
        [HttpPost("send-notification-email-job")]
        public async Task<IActionResult> SendNotifcationEmailJob()
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationTaskDeadline();
            return Ok();
        }
    }
}
