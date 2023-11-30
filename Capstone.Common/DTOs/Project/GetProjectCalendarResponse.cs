using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Project
{
    public class GetProjectCalendarResponse
    {
        public int TotalTask { get; set; }
        public DateTime? DateTime { get; set; }
        public List<GetProjectTasksResponse> TasksByDay { get; set; }
    }
}
