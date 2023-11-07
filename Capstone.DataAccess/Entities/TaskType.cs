using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class TaskType
    {
        [Key] public Guid TypeId { get; set; }
        public string Title { get; set; }
        public List<Task> Tickets { get; set; }
    }
}