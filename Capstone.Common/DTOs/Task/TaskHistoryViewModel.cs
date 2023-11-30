namespace Capstone.Common.DTOs.Task
{
	public class TaskHistoryViewModel
	{
		public Guid HistoryId { get; set; }
		public string Title { get; set; }
		public string ChangeAt { get; set; }
		public Guid TaskId { get; set; }
	}
}
