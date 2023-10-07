namespace Capstone.Common.DTOs.User
{
	public class UpdateUserToken
	{
		public string? RefreshToken { get; set; }
		public DateTime? TokenCreated { get; set; }
		public DateTime? TokenExpires { get; set; }
	}
}
