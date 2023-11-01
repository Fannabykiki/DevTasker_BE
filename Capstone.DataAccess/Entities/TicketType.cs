using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class TicketType
    {
        [Key] public Guid TypeId { get; set; }
        public string Title { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}