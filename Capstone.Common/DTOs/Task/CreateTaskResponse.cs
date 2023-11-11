using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.Task;

public class CreateTaskResponse : BaseResponse
{
    public Guid TaskId { get; set; }
    public string Title  { get; set; }
    public string? Decription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreateTime{ get; set; }
    public DateTime? DeleteAt { get; set; }
    public bool? IsDelete { get; set; }
    public Guid AssignTo { get; set; }
    public Guid CreateBy { get; set; }
    public Guid TypeId { get; set; } // 1 task has many type 
    public Guid? PrevId { get; set; }
    public List<TaskStatus> TaskStatus { get; set; } // just only 1 status 
    public Guid PriorityId { get; set; }
    public Guid InterationId { get; set; }
}