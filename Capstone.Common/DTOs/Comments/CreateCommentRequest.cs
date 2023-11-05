using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Comments
{
    public class CreateCommentRequest
    {
        public string? Content { get; set; }
        public Guid TaskId { get; set; }
        public Guid CreateBy { get; set; }
    }
}
