using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Notification.Abstractions;
using Bootstrap.Components.Notification.Abstractions.Infrastructures.Email;
using Bootstrap.Components.Notification.Abstractions.Models.Constants;
using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using Bootstrap.Components.Notification.Abstractions.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Bootstrap.Components.Notification.Implementations.Email
{
    public class EmailSender : AbstractNotifier<MimeMessage>, IEmailNotifier, IEmailSender
    {
        private readonly IOptions<EmailOptions> _options;

        public EmailSender(MessageService messageService, IOptions<EmailOptions> options) : base(messageService)
        {
            _options = options;
        }

        #region IEmailSender

        public string DefaultSender => _options.Value.DefaultFromAddress;

        async Task IEmailSender.Send(MimeMessage message)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Value.Host, _options.Value.Port, _options.Value.UseSsl);
            await client.AuthenticateAsync(_options.Value.UserName, _options.Value.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        #endregion

        #region AbstractNotifier

        protected override void PopulateMessageRawData(Message message)
        {
            message.PopulateMimeMessage();
        }

        protected override Task SendCore(MimeMessage message) => ((IEmailSender) this).Send(message);
        public override NotificationType Type => NotificationType.Email;
        protected IEnumerable<Message> ConvertToMessages(MimeMessage message) => message.ToMessages();
        protected override MimeMessage ConvertToMessageSendData(Message message) => message.ToMimeMessage();

        #endregion

        #region IEmailNotifier

        public Task Send(MimeMessage message, DateTime sendDt) => Send(message, (DateTime?) sendDt);

        public Task Send(MimeMessage message) => Send(message, null);

        private async Task Send(MimeMessage message, DateTime? sendDt)
        {
            var messages = ConvertToMessages(message).Select(m =>
            {
                m.ScheduleDt = sendDt;
                return m;
            });
            await Task.WhenAll(messages.Select(Send));
        }

        #endregion
    }
}