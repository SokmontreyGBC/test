using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using SendGrid.Helpers.Mail;
using SendGrid;
namespace Assignment1.Services;

public class EmailSender(IConfiguration configuration): IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var from = new MailboxAddress("Brother's Oats Team", "sythatsokmontrey@gmail.com");
            var to = new MailboxAddress("", email);

            var msg = new MimeMessage();
            msg.From.Add(from);
            msg.To.Add(to);
            msg.Subject = subject;

            msg.Body = new BodyBuilder
            {
                TextBody = "Brother's Oats Team",
                HtmlBody = message
            }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp-relay.brevo.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(configuration["Brevo:Account"], configuration["Brevo:MasterKey"]);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending email: {ex.Message}");
            throw;
        }

    }
}
