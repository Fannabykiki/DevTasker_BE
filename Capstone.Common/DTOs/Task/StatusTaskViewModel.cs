using Capstone.Common.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace Capstone.Common.DTOs.Task
{
	public class StatusTaskViewModel
	{
		public Guid BoardStatusId { get; set; }
		public string Title { get; set; }
		public Guid BoardId { get; set; }
		public int? Order { get; set; }
        public BaseResponse BaseResponse { get; set; }
    }
}
