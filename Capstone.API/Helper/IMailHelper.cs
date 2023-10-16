namespace Capstone.API.Helper
{
    public interface IMailHelper
    {
        Task Send(string to, string subject, string body);
    }
}
