namespace Capstone.Common.DTOs.Email
{
	public class ForgotPasswordRequest
	{
        public string To { get; set; }
        public string ResetToken { get; set; }
    }
}
