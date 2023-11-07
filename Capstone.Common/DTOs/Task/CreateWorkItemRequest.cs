using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.Task
{
    public class CreateWorkItemRequest
    {
       
        public string Title { get; set; }
        public string? Decription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid AssignTo { get; set; }
        public Guid CreateBy { get; set; }
        public Guid PriorityId { get; set; }
    }
}
