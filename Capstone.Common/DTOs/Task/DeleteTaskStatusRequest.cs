namespace Capstone.Common.DTOs.Task
{
	public class DeleteTaskStatusRequest
	{
        public Guid TaskStatusId { get; set; }
        public Guid MemberId { get; set; }

    }
}
