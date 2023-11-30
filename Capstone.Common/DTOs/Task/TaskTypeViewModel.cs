using System.ComponentModel.DataAnnotations;

namespace Capstone.Common.DTOs.Task
{
	public class TaskTypeViewModel
	{
		public Guid TypeId { get; set; }
		public string Title { get; set; }
	}
}
