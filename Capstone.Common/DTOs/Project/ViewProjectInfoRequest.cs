using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.Project
{
    public class ViewProjectInfoRequest
    {
        public Guid ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public StatusEnum ProjectStatus { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateAt { get; set; }
        public bool PrivacyStatus { get; set; } // false: Private , true: Public
        public List<ViewMemberProject>? ProjectMembers { get; set; } // 1 project has many member
    }
}
