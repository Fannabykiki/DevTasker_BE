﻿using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Attachment
    {
        [Key]
        public Guid AttachmentId { get; set; }
        public string Title { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? ExprireTime { get; set; }
        public Guid? TaskId { get; set; }
        public Guid CreateBy { get; set; }
        public bool? IsDeleted { get; set; }
        public ProjectMember ProjectMember { get; set; }
        public Task Task { get; set; } // 1 Attachment has 1 comment
    }
}
