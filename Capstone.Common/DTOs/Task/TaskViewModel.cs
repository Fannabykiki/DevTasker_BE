using Microsoft.VisualBasic;
using System.Net.Mail;

namespace Capstone.Common.DTOs.Task
{
	public class TaskViewModel
	{
		public Guid TaskId { get; set; }
		public string Title { get; set; }
		public string? Decription { get; set; }
		public string StartDate { get; set; }
		public string DueDate { get; set; }
		public string ExpireTime { get; set; }
		public string CreateTime { get; set; }
		public string? DeleteAt { get; set; }
		public bool? IsDelete { get; set; }
		public string AssignTo { get; set; }
		public string CreateBy { get; set; }
		public string TypeName { get; set; }
		public string? StatusName { get; set; }
		public Guid? StatusId { get; set; }
		public Guid? TypeId { get; set; }
		public string PriorityName { get; set; }
		public string InterationName { get; set; }
        public List<TaskViewModel> SubTask { get; set; }
    }
}
