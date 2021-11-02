using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Email Sender Creation Steps:
// 1. Create an API key at sendgrid.com
// 2. Copy API key and put it in appsettings.json
// 3. Install SendGrid NuGet Package
// 4. Create EmailOptions and EmailSender
// 5. Configure EmailSender and Options in Startup.cs
// 6. Uncomment EmailSender code in Register PageModel
// 7. Create code to check if user email is confirmed upon sign-in within the Login PageModel
// 8. Create VerifyEmail page that the user is redirected to if their email is not confirmed

namespace SparkAutoPractice.Email
{
    public class EmailSender : IEmailSender
    {
        public EmailOptions Options { get; set; }

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("jacobswink1@gmail.com", "Spark Auto"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));

            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                
            }

            return null;
        }
    }
}
