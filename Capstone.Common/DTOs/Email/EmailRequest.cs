namespace Capstone.Common.DTOs.Email
{
	public class EmailRequest
	{
		public string To { get; set; } = string.Empty;
		public string VerifyToken { get; set; } = string.Empty;
	}
}
