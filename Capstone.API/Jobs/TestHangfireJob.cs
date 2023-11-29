namespace Capstone.API.Jobs
{
    public class TestHangfireJob : IEmailJob
    {
        public Task RunJob()
        {
            Console.WriteLine($"Job executed at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
