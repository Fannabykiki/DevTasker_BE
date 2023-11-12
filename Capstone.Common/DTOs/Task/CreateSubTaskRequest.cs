namespace Capstone.Common.DTOs.Task
{
	public class CreateSubTaskRequest
	{
        public Guid TaskId { get; set; }
        public string Title { get; set; }
		public string? Decription { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime DueDate { get; set; }
		public Guid AssignTo { get; set; }
		public Guid PriorityId { get; set; }
		public Guid TypeId { get; set; }
		public Guid ProjectId { get; set; }
		public Guid StatusId { get; set; }
	}
}
