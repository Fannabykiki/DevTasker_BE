
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.TaskType;
using Capstone.Common.DTOs.User;

namespace Capstone.Common.DTOs.Project
{
    public class GetProjectTasksResponse
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string? Decription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? DeleteAt { get; set; }
        public UserResponse AssignTo { get; set; }
        public UserResponse CreateBy { get; set; }
        public string TaskType { get; set; } 
        public Guid? PrevId { get; set; }
        public Guid? StatusId { get; set; }
        public string TaskStatus { get; set; }
        public string Priority { get; set; }
        public string Interation { get; set; }
    }
}
