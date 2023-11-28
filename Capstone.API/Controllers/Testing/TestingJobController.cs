using Capstone.API.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers.Testing
{
    [Route("api/job-testing")]
    [ApiController]
    public class TestingJobController : ControllerBase
    {
        private readonly IEmailJob _emailJob;
        public TestingJobController(IEmailJob emailJob)
        {
            _emailJob = emailJob;
        }
        [HttpGet("job-run")]
        public async Task<IActionResult> GetJobRun()
        {
            await _emailJob.RunJob();
            return Ok();
        }
    }
}
