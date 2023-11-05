using Capstone.Common.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Comments
{
    public class GetCommentResponse
    {
        public Guid CommentId { get; set; }
        public string? Content { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public Guid TaskId { get; set; }
        public GetUserCommentResponse CreateByUser { get; set; } // 1 comment just create by 1 user
    }
}
