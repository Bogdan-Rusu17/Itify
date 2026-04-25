using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Itify.BusinessService.Infrastructure;

public class MailService(IOptions<MailConfiguration> options, ILogger<MailService> logger)
{
    private readonly MailConfiguration _config = options.Value;

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        if (!_config.MailEnable) return;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Itify", _config.MailAddress));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_config.MailHost, _config.MailPort, SecureSocketOptions.Auto);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(_config.MailUser, _config.MailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send mail to {To} with subject '{Subject}'", to, subject);
        }
    }
}
