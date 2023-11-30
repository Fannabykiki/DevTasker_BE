using Capstone.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Task
    {
        [Key]
        public Guid TaskId { get; set; }
        public string Title  { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreateTime{ get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? ExprireTime { get; set; }
        public bool? IsDelete { get; set; }
        public Guid AssignTo { get; set; }
        public Guid CreateBy { get; set; }
        public ProjectMember ProjectMember { get; set; } // 1 task just create by 1 user, assign to 1 user 
        public Guid TypeId { get; set; } // 1 task has many type 
        public TaskType TicketType { get; set; }
        public Guid? PrevId { get; set; }
        public Guid StatusId { get; set; }
		public List<TaskComment> TaskComments { get; set; } // 1 task has many type 
        public BoardStatus Status { get; set; } // just only 1 status 
        public List<TaskHistory> TaskHistories { get; set; } // many history of change
        public List<Attachment> Attachments { get; set; } // many history of change
        public Guid PriorityId { get; set; }
        public PriorityLevel PriorityLevel { get; set; } // has 1 priority level 
        public Guid InterationId { get; set; }
        public Interation Interation  { get; set; } // has many attachments
    }
}
