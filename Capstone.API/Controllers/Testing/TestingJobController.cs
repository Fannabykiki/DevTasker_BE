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
        private readonly IFailedJob _failedJob;
        public TestingJobController(IEmailJob emailJob, INotificationService notificationService, IFailedJob failedJob)
        {
            _emailJob = emailJob;
            _notificationService = notificationService;
            _failedJob = failedJob;
        }
        [HttpGet("job-run")]
        public async Task<IActionResult> GetJobRun()
        {
            await _emailJob.RunJob();
            return Ok();
        }
        [HttpGet("job-overdue-run")]
        public async Task<IActionResult> GetJobOverdueRun()
        {
            await _failedJob.RunJob();
            return Ok();
        }
        [HttpPost("send-notification-project")]
        public async Task<IActionResult> SendNotifcationProject(Guid projectId)
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationChangeProjectStatus(projectId, userId);
            return Ok();
        }
        [HttpPost("send-notification-task")]
        public async Task<IActionResult> SendNotifcationTask(Guid taskId)
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationChangeTaskStatus(taskId, userId);
            return Ok();
        }
        [HttpPost("send-notification-comment")]
        public async Task<IActionResult> SendNotifcationCommentTask(Guid commentTaskId, string action)
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationCommentTask(commentTaskId, userId, action);
            return Ok();
        }
        [HttpPost("send-notification-email-job")]
        public async Task<IActionResult> SendNotifcationEmailJob()
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationTaskDeadline();
            return Ok();
        }
        [HttpPost("test-notification-failed-task")]
        public async Task<IActionResult> SendNotifcationEmailJobForFailedJob(Guid taskId)
        {
            var userId = this.GetCurrentLoginUserId();
            await _notificationService.SendNotificationTaskFailedDeadline(taskId,  Guid.Parse("AFA06CDD-7713-4B81-9163-C45556E4FA4C"));
            return Ok();
        }
    }
}
