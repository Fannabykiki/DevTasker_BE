using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Attachment
    {
        [Key]
        public Guid AttachmentId { get; set; }
        public string Title { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public Guid? TaskId { get; set; } //2
        public Guid? SubTaskId { get; set; } //2
        public Guid? CommentId { get; set; }
        public Guid CreateBy { get; set; }
        public ProjectMember ProjectMember { get; set; }
        public Task Task { get; set; } //1 Attachment just only in 1 task
        public SubTask SubTask { get; set; } //1 Attachment just only in 1 task
        public TaskComment TaskComment { get; set; } // 1 Attachment has 1 comment
    }
}
