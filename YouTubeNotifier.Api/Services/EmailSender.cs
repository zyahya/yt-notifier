using MailKit.Net.Smtp;
using MailKit.Security;

using MimeKit;

namespace YouTubeNotifier.Api.Services;

public class EmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<SmtpOptions> options, ILogger<EmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
    string to,
    string subject,
    string html)
    {
        _logger.LogInformation("Sending email with subject {Subject} to {Recipient}.", subject, to);

        try
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

            _logger.LogInformation("Email sent successfully to {Recipient}.", to);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to send email with subject {Subject} to {Recipient}.", subject, to);
            throw;
        }
    }
}
