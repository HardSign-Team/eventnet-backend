namespace Eventnet.Domain;

public interface IEmailService
{
    Task SendEmailAsync(string userEmail, string subject, string message);
}