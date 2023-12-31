﻿using Capstone.Common.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class TaskComment
    {
        [Key]
        public Guid CommentId { get; set; }
        public string? Content { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? ReplyTo { get; set; }
        public Task? Task  { get; set; } // 1 comment just only in 1 task
        public Guid CreateBy { get; set; }
        public ProjectMember ProjectMember { get; set; } // 1 comment just create by 1 user
    }
}
