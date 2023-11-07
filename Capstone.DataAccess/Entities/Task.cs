﻿using Capstone.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Task
    {
        [Key]
        public Guid TaskId { get; set; }
        public string Title  { get; set; }
        public string? Decription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreateTime{ get; set; }
        public DateTime? DeleteAt { get; set; }
        public bool? IsDelete { get; set; }
        public Guid AssignTo { get; set; }
        public Guid CreateBy { get; set; }
        public ProjectMember ProjectMember { get; set; } // 1 task just create by 1 user, assign to 1 user 
        public Guid TypeId { get; set; } // 1 task has many type 
        public TaskType TicketType { get; set; }
        public Guid? PrevId { get; set; }
        public List<TaskComment> TaskComments { get; set; } // 1 task has many type 
        public Guid StatusId { get; set; }
        public Status Status { get; set; } // just only 1 status 
        public List<TaskHistory> TaskHistories { get; set; } // many history of change
        public Guid PriorityId { get; set; }
        public PriorityLevel PriorityLevel { get; set; } // has 1 priority level 
        public List<Attachment> Attachments { get; set; } // has many attachments
        public Guid InterationId { get; set; }
        public Interation Interation  { get; set; } // has many attachments
    }
}