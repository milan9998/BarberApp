using Hair.Application.Common.Configuration;
using Hair.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Hair.Infrastructure.Services;

public class SmtpEmailService(
    IOptions<EmailSettings> emailOptions,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            throw new ArgumentException("Recipient email is required.", nameof(toEmail));
        }

        if (string.IsNullOrWhiteSpace(_settings.Username) || string.IsNullOrWhiteSpace(_settings.Password))
        {
            throw new InvalidOperationException("SMTP nije konfigurisan (Email:Username/Password).");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        Exception? lastError = null;
        var attempts = new (string Host, int Port, SecureSocketOptions Options)[]
        {
            (_settings.Host, _settings.Port, SecureSocketOptions.StartTls),
            (_settings.Host, 465, SecureSocketOptions.SslOnConnect),
            ("smtp.zoho.eu", 587, SecureSocketOptions.StartTls),
            ("smtp.zoho.eu", 465, SecureSocketOptions.SslOnConnect),
            ("smtppro.zoho.com", 587, SecureSocketOptions.StartTls)
        };

        foreach (var attempt in attempts.DistinctBy(a => $"{a.Host}:{a.Port}:{a.Options}"))
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(attempt.Host, attempt.Port, attempt.Options, cancellationToken);
                await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                logger.LogInformation("Email sent to {Email} via {Host}:{Port}", toEmail, attempt.Host, attempt.Port);
                await client.DisconnectAsync(true, cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                lastError = ex;
                logger.LogWarning(ex, "SMTP attempt failed on {Host}:{Port}", attempt.Host, attempt.Port);
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, cancellationToken);
                }
            }
        }

        throw new InvalidOperationException(
            "Slanje emaila nije uspelo (Zoho SMTP autentifikacija). Proverite App Password u Zoho Mailu.",
            lastError);
    }
}
