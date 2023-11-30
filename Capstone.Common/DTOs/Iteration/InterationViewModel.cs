namespace Capstone.Common.DTOs.Iteration
{
	public class InterationViewModel
    {
		public Guid InterationId { get; set; }
		public string InterationName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Guid BoardId { get; set; }
		public Guid StatusId { get; set; }
		public string StatusName { get; set; }
	}
}
