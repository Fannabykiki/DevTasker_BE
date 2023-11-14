using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.Task;

public class CreateTaskResponse
{
	public Guid TaskId { get; set; }
    public string Title  { get; set; }
    public string? Description { get; set; }
    public string StartDate { get; set; }
    public string DueDate { get; set; }
    public string CreateTime{ get; set; }
    public string? DeleteAt { get; set; }
    public bool? IsDelete { get; set; }
    public Guid AssignTo { get; set; }
    public Guid CreateBy { get; set; }
    public Guid TypeId { get; set; } // 1 task has many type 
    public string? Status { get; set; }
    public Guid? StatusId { get; set; }
    public Guid PriorityId { get; set; }
    public Guid InterationId { get; set; }
    public BaseResponse BaseResponse { get; set; }
}