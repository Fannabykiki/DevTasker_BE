using Capstone.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class TaskHistory
    {
        [Key]
        public Guid HistoryId { get; set; }
        public string Title { get; set; }
        public DateTime? ChangeAt { get; set; }
        public Guid? ChangeBy { get; set; }
        public Guid PreviousStatusId { get; set; } //to do
        public Guid CurrentStatusId { get; set; } //to do => in progess FE 
        public Status TaskStatus { get; set; }
        public Guid TaskId { get; set; }
        public Task Task { get; set; } // 1 history just only 1 task
        public ProjectMember ProjectMember { get; set; } // 1 history just only 1 task
    }
}
