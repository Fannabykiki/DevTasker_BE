using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.TicketComment
{
    public class CreateTicketComment
    {
        public string? Content { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid TaskId { get; set; }
        public Guid AttachmentId { get; set; }
        public Guid CreateBy { get; set; }
    }
}
