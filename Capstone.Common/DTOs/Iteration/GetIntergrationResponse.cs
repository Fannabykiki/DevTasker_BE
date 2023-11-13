using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.Iteration
{
    public class GetIntergrationResponse
    {
        public Guid InterationId { get; set; }
        public string InterationName { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Guid BoardId { get; set; }
        public Guid StatusId { get; set; }
        public BaseResponse Response { get; set; }
    }
}
