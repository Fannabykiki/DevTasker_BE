using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Project
{
    public class GetProjectReportRequest
    {
        public ReportRecord reportProject { get; set; }
        public List<ReportRecord>? reportRecordByWeerk { get; set; }
        public List<MemberTasks>? memberTaks { get; set; }


    }
    public class MemberTasks
    {
        public Guid MemberId { get; set; }
        public Guid UserId { get; set; }
        public string? Fullname { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsOwner { get; set; }
        public int TotalTasks { get; set; }
        public List<ReportStatus>? reportStatuses { get; set; }
    }
    public class ReportRecord 
    {
        public int TotalTask { get; set; }
        public DateTime? DateTime { get; set; }
        public List<ReportStatus>? reportStatuses { get; set; }
    }
    public class ReportStatus
    {
        public Guid BoardStatusId { get; set; }
        public string Title { get; set; }
        public int? NumberTask { get; set; }
        public int? Percent { get; set; }

    }
}
