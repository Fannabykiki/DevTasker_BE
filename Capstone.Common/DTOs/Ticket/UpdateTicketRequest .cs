using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.Task
{
    public class UpdateTicketRequest
    {
       
        public string Title { get; set; }
        public string? Decription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public Guid AssignTo { get; set; }
        public TicketTypeEnum TicketType { get; set; }
        public Guid PriorityId { get; set; }
        public TaskStatusEnum TicketStatus { get; set; }
        public Guid InterationId { get; set; }
    }
}
