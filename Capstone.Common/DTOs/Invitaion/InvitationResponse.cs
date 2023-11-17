namespace Capstone.Common.DTOs.Invitaion
{
	public class InvitationResponse
	{
		public Guid InvitationId { get; set; }
		public string InviteBy { get; set; }
		public string ProjectName { get; set; }
		public Guid? ProjectId { get; set; }
		public Guid StatusId { get; set; }
		public string StatusName { get; set; }
		public string InviteTo { get; set; }
		public DateTime CreateAt { get; set; }
	}
}
