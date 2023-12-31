﻿namespace Capstone.Common.DTOs.Task
{
    public class UpdateTaskRequest
    {
        public Guid TaskId { get; set; }
        public Guid InterationId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public Guid AssignTo { get; set; }
        public Guid TypeId{ get; set; }
        public Guid PriorityId { get; set; }
        public Guid StatusId { get; set; }
        public Guid MemberId { get; set; }
    }
}
