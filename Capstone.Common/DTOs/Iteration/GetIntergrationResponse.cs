namespace Capstone.Common.DTOs.Iteration
{
    public class GetIntergrationResponse
    {
        public Guid InterationId { get; set; }
        public string InterationName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid BoardId { get; set; }
    }
}
