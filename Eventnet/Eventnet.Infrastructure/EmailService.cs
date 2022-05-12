using Eventnet.Domain;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Eventnet.Infrastructure;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration emailConfiguration;
    private readonly MailboxAddress senderAddress;

    public EmailService(EmailConfiguration emailConfiguration)
    {
        this.emailConfiguration = emailConfiguration;
        senderAddress = new MailboxAddress("Администрация сайта", emailConfiguration.CompanyAddress);
    }

    public async Task SendEmailAsync(string userEmail, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(senderAddress);
        emailMessage.To.Add(new MailboxAddress("", userEmail));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = message
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(emailConfiguration.Host, emailConfiguration.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailConfiguration.Login, emailConfiguration.Password);
        await client.SendAsync(emailMessage);

        await client.DisconnectAsync(true);
    }
}