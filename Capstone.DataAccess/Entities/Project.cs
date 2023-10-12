using Capstone.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
		public string Description { get; set; }
		public StatusEnum ProjectStatus { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public bool PrivacyStatus { get; set; } // false: Private , true: Public
        public Board Board { get; set; }
		public List<ProjectSchemas> ProjectSchemas { get; set; }
        public List<ProjectMember> ProjectMembers { get; set; } // 1 project has many member
    }
}
