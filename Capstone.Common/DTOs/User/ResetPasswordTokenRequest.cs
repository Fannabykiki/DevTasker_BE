namespace Capstone.Common.DTOs.User
{
	public class ResetPasswordTokenRequest
	{
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
