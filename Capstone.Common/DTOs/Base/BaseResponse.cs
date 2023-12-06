namespace Capstone.Common.DTOs.Base
{
    public class BaseResponse
    {
        public int StatusCode { get; set; }
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
    }
}
