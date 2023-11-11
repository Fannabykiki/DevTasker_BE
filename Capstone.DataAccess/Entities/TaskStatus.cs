using System.ComponentModel.DataAnnotations;

namespace Capstone.DataAccess.Entities;

public class TaskStatus
{
    public Guid TaskId { get; set; }
    public Task Task { get; set; }
    public Guid StatusId { get; set; }
    public Status Status { get; set; }
}