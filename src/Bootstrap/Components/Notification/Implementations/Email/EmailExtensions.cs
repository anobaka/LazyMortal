using System;
using System.Collections.Generic;
using System.IO;
using Bootstrap.Components.Notification.Abstractions.Models.Constants;
using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using Bootstrap.Extensions;
using MimeKit;
using Newtonsoft.Json;

namespace Bootstrap.Components.Notification.Implementations.Email
{
    public static class EmailExtensions
    {
        public static string ToBase64String(this MimeMessage message)
        {
            using var ms = new MemoryStream();
            message.WriteTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return Convert.ToBase64String(ms.ToArray());
        }

        public static MimeMessage LoadMimeMessageFromBase64String(string str)
        {
            using var ms = new MemoryStream();
            var bytes = Convert.FromBase64String(str);
            ms.Write(bytes);
            ms.Seek(0, SeekOrigin.Begin);
            var mm = MimeMessage.Load(ms);
            return mm;
        }

        public static MimeMessage ToMimeMessage(this Message message)
        {
            if (message.RawData is MimeMessage mm)
            {
                return mm;
            }

            if (message.RawDataString.IsNotEmpty())
            {
                mm = LoadMimeMessageFromBase64String(message.RawDataString);
                return mm;
            }

            return new MimeMessage
            {
                Subject = message.Subject,
                Body = new TextPart(message.Content),
                From = {new MailboxAddress(message.Sender, message.Sender)},
                To = {MailboxAddress.Parse(message.Receiver)}
            };
        }

        public static void PopulateMimeMessage(this Message message)
        {
            var mm = message.ToMimeMessage();
            message.RawData ??= mm;

            if (message.RawDataString.IsNullOrEmpty())
            {
                message.RawDataString = mm.ToBase64String();
            }
        }

        public static IEnumerable<Message> ToMessages(this MimeMessage mm)
        {
            foreach (var receiver in mm.To)
            {
                foreach (var sender in mm.From)
                {
                    yield return new Message
                    {
                        Content = JsonConvert.SerializeObject(mm),
                        Receiver = receiver.ToString(),
                        Sender = sender.ToString(),
                        Subject = mm.Subject,
                        Type = NotificationType.Email,
                        RawDataString = mm.ToBase64String(),
                        RawData = mm
                    };
                }
            }
        }
    }
}