using Capstone.Common.Enums;

namespace Capstone.DataAccess.Entities
{
	public class Interation
	{
		public Guid InterationId { get; set; }
		public string InterationName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
        public Guid BoardId { get; set; }
        public Guid StatusId { get; set; }
        public Status Status { get; set; }
        public Board Board { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
