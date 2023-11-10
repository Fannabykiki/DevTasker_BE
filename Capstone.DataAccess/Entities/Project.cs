using Capstone.Common.DTOs.Project;
using Capstone.Common.Enums;
using Capstone.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
		public string? Description { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid SchemasId { get; set; }
        public Guid StatusId { get; set; }
        public DateTime? DeleteAt { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? ExpireAt { get; set; }
        public bool PrivacyStatus { get; set; } // false: Private , true: Public
        public List<ProjectMember> ProjectMembers { get; set; } // 1 project has many member
		public Board Board { get; set; }
		public Status Status { get; set; }
		public Schema Schemas { get; set; }
    }
}