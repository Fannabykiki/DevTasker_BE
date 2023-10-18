using Capstone.Common.Enums;

namespace Capstone.DataAccess.Entities
{
	public class Interation
	{
		public Guid InterationId { get; set; }
		public string InterationName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
        public Guid ProjectId { get; set; }
        public InterationStatusEnum Status { get; set; }
        public Project Project { get; set; }
        public List<Board> Boards { get; set; }
    }
}
