using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Notification.Abstractions.Infrastructures;
using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using Bootstrap.Components.Notification.Abstractions.Models.RequestModels;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Abstractions.Services
{
    [Obsolete]
    public class MessageService: ResourceService<NotificationDbContext, Message, int>
    {
        private IEnumerable<INotifier> Notifiers => GetRequiredService<IEnumerable<INotifier>>();

        public MessageService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<SearchResponse<Message>> Search(MessageSearchRequestModel model)
        {
            return base.Search(t =>
                    (string.IsNullOrEmpty(model.Receiver) || model.Receiver == t.Receiver) &&
                    (model.Types == null || model.Types.Length == 0 || model.Types.Contains(t.Type)), model.PageIndex,
                model.PageSize);
        }

        public async Task Send(CommonMessageSendRequestModel model)
        {
            var notifiers = Notifiers.Where(a => model.Types.HasFlag(a.Type)).ToArray();
            var messages = notifiers.Select(n =>
            {
                var m = new Message
                {
                    Receiver = model.Receiver,
                    Content = model.Content,
                    Subject = model.Subject,
                    Sender = model.Sender,
                    Type = n.Type,
                    ScheduleDt = model.ScheduleDt
                };
                return m;
            }).ToDictionary(a => a.Type, a => a);
            await Task.WhenAll(notifiers.Select(n => n.Send(messages[n.Type])));
        }
    }
}