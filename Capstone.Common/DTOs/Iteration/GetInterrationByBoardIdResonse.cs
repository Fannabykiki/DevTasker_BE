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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid BoardId { get; set; }
        public List<WorkItemResponse>? workItemResponses { get; set; }
    }

    public class WorkItemResponse
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string TaskType { get; set; }
        public Guid? StatusId { get; set; }
        public string TaskStatus { get; set; }
        public List<TaskResponse> Tasks { get; set; }
    }

    public class TaskResponse
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string TaskType { get; set; }
        public Guid? StatusId { get; set; }
        public string TaskStatus { get; set; }
    }

   
}
