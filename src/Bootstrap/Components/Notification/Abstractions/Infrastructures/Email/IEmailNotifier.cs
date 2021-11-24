using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;

namespace Bootstrap.Components.Notification.Abstractions.Infrastructures.Email
{
    public interface IEmailNotifier : INotifier
    {
        string DefaultSender { get; }

        Task Send(string subject, string textBody, string toAddress, DateTime sendDt) =>
            Send(subject, textBody, DefaultSender, DefaultSender, toAddress, sendDt);

        Task Send(string subject, string textBody, string sender, string senderName, string toAddress,
            DateTime sendDt) => Send(subject,
            new TextPart(textBody),
            new List<InternetAddress> {new MailboxAddress(senderName, sender)},
            new List<InternetAddress> {MailboxAddress.Parse(toAddress)}, sendDt);

        Task Send(string subject, MimeEntity body, List<InternetAddress> fromList,
            List<InternetAddress> toList, DateTime sendDt)
        {
            var message = new MimeMessage
            {
                Subject = subject,
                Body = body
            };
            message.From.AddRange(fromList);
            message.To.AddRange(toList);
            return Send(message, sendDt);
        }

        Task Send(MimeMessage message, DateTime sendDt);

        Task Send(string subject, string textBody, string toAddress) =>
            Send(subject, textBody, DefaultSender, DefaultSender, toAddress);

        Task Send(string subject, string textBody, string sender, string senderName, string toAddress) => Send(subject,
            new TextPart(textBody),
            new List<InternetAddress> {new MailboxAddress(senderName, sender)},
            new List<InternetAddress> {MailboxAddress.Parse(toAddress)});

        Task Send(string subject, MimeEntity body, List<InternetAddress> fromList,
            List<InternetAddress> toList)
        {
            var message = new MimeMessage
            {
                Subject = subject,
                Body = body
            };
            message.From.AddRange(fromList);
            message.To.AddRange(toList);
            return Send(message);
        }

        Task Send(MimeMessage message);
    }
}