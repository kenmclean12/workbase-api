using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace newproj.Services.Emails;

public class ResendMailKitSender(
    IConfiguration configuration,
    IHostEnvironment environment) : ISender
{
    public async Task<SendResponse> SendAsync(
        IFluentEmail email,
        CancellationToken? token = null)
    {
        var response = new SendResponse();
        var cancellationToken = token ?? CancellationToken.None;

        try
        {
            var data = email.Data;
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    data.FromAddress.Name,
                    data.FromAddress.EmailAddress));

            foreach (var address in data.ToAddresses)
                message.To.Add(new MailboxAddress(address.Name, address.EmailAddress));

            foreach (var address in data.CcAddresses)
                message.Cc.Add(new MailboxAddress(address.Name, address.EmailAddress));

            foreach (var address in data.BccAddresses)
                message.Bcc.Add(new MailboxAddress(address.Name, address.EmailAddress));

            foreach (var address in data.ReplyToAddresses)
                message.ReplyTo.Add(new MailboxAddress(address.Name, address.EmailAddress));

            message.Subject = data.Subject ?? string.Empty;

            var body = new BodyBuilder();

            if (data.IsHtml)
                body.HtmlBody = data.Body;
            else
                body.TextBody = data.Body;

            if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
                body.TextBody = data.PlaintextAlternativeBody;

            foreach (var attachment in data.Attachments)
            {
                body.Attachments.Add(
                    attachment.Filename,
                    attachment.Data,
                    ContentType.Parse(attachment.ContentType));
            }

            message.Body = body.ToMessageBody();

            using var client = new SmtpClient();

            client.CheckCertificateRevocation = !environment.IsDevelopment();

            await client.ConnectAsync(
                configuration["Email:Host"],
                configuration.GetValue<int>("Email:Port"),
                SecureSocketOptions.StartTls,
                cancellationToken);

            await client.AuthenticateAsync(
                configuration["Email:Username"],
                configuration["Email:Password"],
                cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            response.ErrorMessages.Add(ex.Message);
        }

        return response;
    }

    public SendResponse Send(
        IFluentEmail email,
        CancellationToken? token = null)
    {
        return SendAsync(email, token).GetAwaiter().GetResult();
    }
}
