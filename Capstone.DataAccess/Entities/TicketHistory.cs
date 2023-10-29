using Capstone.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class TicketHistory
    {
        [Key]
        public Guid HistoryId { get; set; }
        public string Title { get; set; }
        public DateTime? ChangeAt { get; set; }
        public Guid? PreviousStatusId { get; set; } //to do
        public Guid CurrentStatusId { get; set; } //to do => in progess FE 
        public List<Status> TaskStatus { get; set; }
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; } // 1 history just only 1 task
    }
}
