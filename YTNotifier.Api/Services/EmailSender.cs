using MailKit.Net.Smtp;
using MailKit.Security;

using MimeKit;

namespace YTNotifier.Api.Services;

public class EmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public EmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(
    string to,
    string subject,
    string html)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _options.DisplayName,
            _options.Email));

        message.To.Add(MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = html
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _options.Host,
            _options.Port,
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _options.Email,
            _options.Password);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}
