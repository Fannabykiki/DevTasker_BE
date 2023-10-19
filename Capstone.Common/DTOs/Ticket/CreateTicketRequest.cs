using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.Task
{
    public class CreateTicketRequest
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public string? Decription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? DeleteTime { get; set; }
        public Guid AssignTo { get; set; }
        public Guid CreateBy { get; set; }
        public TicketTypeEnum TicketType { get; set; }
        public Guid PriorityId { get; set; }
        public Guid PrevId { get; set; }
        public TaskStatusEnum TicketStatus { get; set; }
        public Guid BoardId { get; set; }
    }
}
