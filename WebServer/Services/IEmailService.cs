namespace WebServer.services
{
    public interface IEmailService
    {
        void SendEmail(string email, string title, string message);
    }
}
