using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using unior_backend_test.Domain.Interfaces;

namespace junior_backend_test.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {


                var smtpClient = new SmtpClient(config["EmailSettings:SmtpServer"])
                {
                    Port = int.Parse(config["EmailSettings:Port"]),
                    Credentials = new NetworkCredential(config["EmailSettings:Username"], config["EmailSettings:Password"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(config["EmailSettings:SenderEmail"], config["EmailSettings:SenderName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
                // Optional: log to file or DB
            }
        }
    }
}
