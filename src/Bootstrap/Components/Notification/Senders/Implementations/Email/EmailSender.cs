using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Components.Notification.Senders;
using Bootstrap.Components.Notification.Senders.Implementations.Email;
using Bootstrap.Models.ResponseModels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Bakabase.OtherWorld.Business.Components.Email
{
    public class EmailSender : IDisposable, IMessageSender, IEmailSender
    {
        private readonly IOptions<EmailOptions> _options;

        public EmailSender(IOptions<EmailOptions> options)
        {
            _options = options;
        }

        public async Task Send(string subject, MimeEntity body, List<InternetAddress> fromList,
            List<InternetAddress> toList)
        {
            var message = new MimeMessage
            {
                Subject = subject,
                Body = body
            };
            message.From.AddRange(fromList);
            message.To.AddRange(toList);
            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Value.Host, _options.Value.Port, _options.Value.UseSsl);
            await client.AuthenticateAsync(_options.Value.UserName, _options.Value.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task Send(string subject, string textBody, string sender, string toAddress)
        {
            await Send(subject, new TextPart(textBody),
                new List<InternetAddress> {new MailboxAddress(sender, _options.Value.DefaultFromAddress)},
                new List<InternetAddress> {MailboxAddress.Parse(toAddress)});
        }

        public async Task Send(string subject, string textBody, string toAddress)
        {
            await Send(subject, textBody, null, toAddress);
        }

        public void Dispose()
        {
        }

        public async Task<BaseResponse> Send(Message message)
        {
            await Send(message.Content, message.Content, message.Receiver);
            return BaseResponseBuilder.Ok;
        }
    }
}