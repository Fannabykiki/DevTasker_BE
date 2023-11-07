namespace Capstone.Common.DTOs.Task
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; }
        public string? Decription { get; set; }
        public DateTime DueDate { get; set; }
        public Guid AssignTo { get; set; }
        public Guid TypeId{ get; set; }
        public Guid PriorityId { get; set; }
        public Guid StatusId { get; set; }
    }
}
