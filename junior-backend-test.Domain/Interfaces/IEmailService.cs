namespace unior_backend_test.Domain.Interfaces
{
    public interface IEmailService
        {
            Task SendEmailAsync(string toEmail, string subject, string body);
        }
}
