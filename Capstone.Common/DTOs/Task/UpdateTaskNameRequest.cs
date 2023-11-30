namespace Capstone.Common.DTOs.Task
{
	public class UpdateTaskNameRequest
	{
        public string Title { get; set; }
        public Guid StatusTaskId  { get; set; }
    }
}
