using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;

namespace Bootstrap.Components.Notification.Senders.Implementations.Email
{
    public interface IEmailSender
    {
        /// <summary>
        /// todo: Use more abstracter things, not MineKit.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="fromList"></param>
        /// <param name="toList"></param>
        /// <returns></returns>
        Task Send(string subject, MimeEntity body, List<InternetAddress> fromList,
            List<InternetAddress> toList);

        Task Send(string subject, string textBody, string sender, string toAddress);
        Task Send(string subject, string textBody, string toAddress);
    }
}