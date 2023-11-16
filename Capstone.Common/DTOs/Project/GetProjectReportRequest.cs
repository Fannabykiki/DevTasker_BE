using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Project
{
    public class GetProjectReportRequest
    {
        public ReportRecord reportRecord { get; set; }
        public List<ReportRecord> reportRecordByWeerk { get; set; }

    }
    public class ReportRecord 
    {
        public int TotalTask { get; set; }
        public DateTime? DateTime { get; set; }
        public List<ReportStatus> reportStatuses { get; set; }
    }
    public class ReportStatus
    {
        public Guid BoardStatusId { get; set; }
        public string Title { get; set; }
        public int? NumberTask { get; set; }
        public int? Percent { get; set; }

    }
}
