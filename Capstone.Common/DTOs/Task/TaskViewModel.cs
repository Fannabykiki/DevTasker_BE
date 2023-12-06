using Microsoft.VisualBasic;
using System.Net.Mail;

namespace Capstone.Common.DTOs.Task
{
	public class TaskViewModel
	{
		public Guid TaskId { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public string StartDate { get; set; }
		public string DueDate { get; set; }
		public string ExpireTime { get; set; }
		public string CreateTime { get; set; }
		public string? DeleteAt { get; set; }
		public bool? IsDelete { get; set; }
		public string AssignTo { get; set; }
		public string CreateBy { get; set; }
		public string TypeName { get; set; }
		public string? StatusName { get; set; }
		public Guid? StatusId { get; set; }
		public Guid? TypeId { get; set; }
		public Guid Priority { get; set; }
		public string PriorityName { get; set; }
		public int PriorityLevel { get; set; }
		public Guid InterationId { get; set; }
		public string InterationName { get; set; }
		public string UserStatus { get; set; }
		public string MemberStatus { get; set; }
		public Guid MemberStatusId { get; set; }
		public Guid UserStatusId { get; set; }
        public int? TotalComment { get; set; }
        public int? TotalAttachment { get; set; }
        public List<SubTask> SubTask { get; set; }
    }

	public class SubTask
	{
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string StartDate { get; set; }
        public string DueDate { get; set; }
        public string ExpireTime { get; set; }
        public string CreateTime { get; set; }
        public string? DeleteAt { get; set; }
        public bool? IsDelete { get; set; }
        public string AssignTo { get; set; }
        public string CreateBy { get; set; }
        public string TypeName { get; set; }
        public string? StatusName { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? TypeId { get; set; }
        public Guid Priority { get; set; }
        public string PriorityName { get; set; }
		public int PriorityLevel { get; set; }
        public Guid InterationId { get; set; }
        public string InterationName { get; set; }
        public int? TotalComment { get; set; }
        public int? TotalAttachment { get; set; }
    }
}
