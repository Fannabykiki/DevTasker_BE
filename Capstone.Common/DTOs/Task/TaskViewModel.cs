using Microsoft.VisualBasic;
using System.Net.Mail;

namespace Capstone.Common.DTOs.Task
{
	public class TaskViewModel
	{
		public Guid TaskId { get; set; }
		public string Title { get; set; }
		public string? Decription { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime DueDate { get; set; }
		public DateTime CreateTime { get; set; }
		public DateTime? DeleteAt { get; set; }
		public bool? IsDelete { get; set; }
		public string AssignTo { get; set; }
		public string CreateBy { get; set; }
		public string TypeName { get; set; }
		public Guid? PrevId { get; set; }
		public string? StatusName { get; set; }
		public string PriorityName { get; set; }
		public string InterationName { get; set; }
        public List<TaskViewModel> SubTask { get; set; }
    }
}
