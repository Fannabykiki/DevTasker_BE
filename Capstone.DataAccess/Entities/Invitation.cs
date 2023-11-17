namespace Capstone.DataAccess.Entities;

public class Invitation
{
    public Guid InvitationId { get; set; }
    public Guid CreateBy { get; set; }
    public ProjectMember ProjectMember { get; set; }
    public Guid StatusId { get; set; }
    public string InviteTo { get; set; }
    public DateTime CreateAt { get; set; }
    public Status Status { get; set; }
}