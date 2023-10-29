using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Iteration
{
    public class GetInterrationByBoardIdResonse
    {
        public Guid InterationId { get; set; }
        public string InterationName { get; set; }
        public string Status { get; set; }
        public List<WorkItemResponse>? workItemResponses { get; set; }
    }

    public class WorkItemResponse
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public string TicketType { get; set; }
        public string TicketStatus { get; set; }
        public List<TicketResponse> Tickets { get; set; }
    }

    public class TicketResponse
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public string TicketType { get; set; }
        public string TicketStatus { get; set; }
    }
}
