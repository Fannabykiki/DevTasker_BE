using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.Ticket
{
    public class CreateTicketRequest
    {
       
        public string Title { get; set; }
        public string? Decription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public Guid AssignTo { get; set; }
        public Guid PriorityId { get; set; }
    }
}
