using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.User;
using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Iteration
{

    public class GetInterrationByBoardIdResonse
    {
        public Guid InterationId { get; set; }
        public string InterationName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid BoardId { get; set; }
        public List<TaskViewModel>? Tasks { get; set; }
    }

    //public class Tasks
    //{
    //    public Guid TaskId { get; set; }
    //    public string Title { get; set; }
    //    public string? Description { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime DueDate { get; set; }
    //    public DateTime CreateTime { get; set; }
    //    public DateTime? DeleteAt { get; set; }
    //    public bool? IsDelete { get; set; }
    //    public UserResponse AssignTo { get; set; }
    //    public UserResponse CreateBy { get; set; }
    //    public string TypeName { get; set; }
    //    public string? StatusName { get; set; }
    //    public Guid? StatusId { get; set; }
    //    public Guid? TypeId { get; set; }
    //    public string PriorityName { get; set; }
    //    public string InterationName { get; set; }
    //    public List<TaskResponse> SubTasks { get; set; }
    //}

    //public class TaskResponse
    //{
    //    public Guid TaskId { get; set; }
    //    public string Title { get; set; }
    //    public string? Description { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime DueDate { get; set; }
    //    public DateTime CreateTime { get; set; }
    //    public DateTime? DeleteAt { get; set; }
    //    public bool? IsDelete { get; set; }
    //    public UserResponse AssignTo { get; set; }
    //    public UserResponse CreateBy { get; set; }
    //    public string TypeName { get; set; }
    //    public string? StatusName { get; set; }
    //    public Guid? StatusId { get; set; }
    //    public Guid? TypeId { get; set; }
    //    public string PriorityName { get; set; }
    //    public string InterationName { get; set; }
    //}

   
}
