using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.Project
{
	public class ChangeProjectStatusRespone
	{
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string StatusName { get; set; }
        public Guid StatusId { get; set; }
        public BaseResponse StatusResponse { get; set; }
	}
}
