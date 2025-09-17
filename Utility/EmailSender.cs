using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace MyCeima.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            var client = new SmtpClient("Smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("adam.elking1123@gmail.com", "mdkd bevt nonl scyu")
                };

            return client.SendMailAsync(
                    new MailMessage(from: "your.email@live.com",
                    to: email,
                    subject,
                    htmlMessage)
                    {
                        IsBodyHtml = true
                    }
                );
        }
    }
}
