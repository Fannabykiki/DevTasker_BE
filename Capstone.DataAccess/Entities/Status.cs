using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Status
    {
        [Key]
        public Guid StatusId { get; set; }
        public string Title { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<User> Users { get; set; }
        public List<Project> Projects { get; set; }
        public List<Interation> Interations { get; set; }
        public List<Board> Boards { get; set; }
        public List<TicketHistory> TaskHistories { get; set; }
    }
}
