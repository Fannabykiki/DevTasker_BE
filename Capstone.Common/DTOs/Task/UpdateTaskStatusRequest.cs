namespace Capstone.Common.DTOs.Task
{
    public class UpdateTaskStatusRequest
    {
        public Guid TaskId { get; set; }
        public Guid MemberId { get; set; }
        public Guid StatusId { get; set; }
    }
}
