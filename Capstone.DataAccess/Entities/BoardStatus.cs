using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
	public class BoardStatus
	{
		[Key]
		public Guid BoardStatusId { get; set; }
		public string Title { get; set; }
		public Guid BoardId { get; set; }
        public int? Order { get; set; }
        public Board Board { get; set; }
        public Guid StatusId { get; set; }
        public Status Status { get; set; }
		public List<Task> Tasks { get; set; }
		public List<TaskHistory> TaskHistory { get; set; }
	}
}
