namespace Capstone.Common.DTOs.Project;

public class InviteUserRequest
{
    public List<string> Email { get; set; }
    public Guid ProjectId { get; set; }
}