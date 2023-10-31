using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.User
{
    public class CreateUserResponse : BaseResponse
    {
        public string? VerifyToken { get; set; }
    }
}
