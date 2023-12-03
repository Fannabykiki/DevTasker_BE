using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.Task
{
	public class UpdateTaskOrderResponse : BaseResponse
	{
		public Guid BoardStatusId { get; set; }
		public string Title { get; set; }
		public Guid BoardId { get; set; }
		public int? Order { get; set; }
	}
}
