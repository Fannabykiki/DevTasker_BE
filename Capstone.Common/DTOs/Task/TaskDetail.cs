namespace Capstone.Common.DTOs.Task
{
	public class TaskDetail
	{
		public Guid TaskId { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime DueDate { get; set; }
		public DateTime CreateTime { get; set; }
		public DateTime? DeleteAt { get; set; }
		public DateTime? ExprireTime { get; set; }
		public bool? IsDelete { get; set; }
		public Guid AssignTo { get; set; }
		public Guid CreateBy { get; set; }
		public Guid TypeId { get; set; } // 1 task has many type 
		public Guid? PrevId { get; set; }
		public Guid StatusId { get; set; }
	}
}
