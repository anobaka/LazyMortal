using System;
using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Components.Notification.Abstractions;
using Bootstrap.Components.Notification.Abstractions.Infrastructures;
using Bootstrap.Components.Notification.Abstractions.Models.Constants;
using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using Bootstrap.Components.Notification.Abstractions.Services;
using Bootstrap.Extensions;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Implementations
{
    public abstract class AbstractNotifier<TMessageData> : INotifier where TMessageData : class
    {
        private readonly MessageService _messageService;

        protected AbstractNotifier(MessageService messageService)
        {
            _messageService = messageService;
        }

        public abstract NotificationType Type { get; }
        protected abstract TMessageData ConvertToMessageSendData(Message message);
        protected abstract Task SendCore(TMessageData messageData);

        protected virtual void PopulateMessageRawData(Message message)
        {
        }

        public async Task<BaseResponse> Send(Message message)
        {
            PopulateMessageRawData(message);
            var isNewMessage = message.Id == 0;
            if (isNewMessage)
            {
                await _messageService.Add(message, true);
            }

            if (!message.ScheduleDt.HasValue || !isNewMessage)
            {
                MessageStatus result;
                string resultMessage = null;
                try
                {
                    var data = ConvertToMessageSendData(message);
                    await SendCore(data);
                    result = MessageStatus.Succeed;
                }
                catch (Exception e)
                {
                    result = MessageStatus.Failed;
                    resultMessage = e.BuildFullInformationText();
                }

                await _messageService.UpdateByKey(message.Id, t =>
                {
                    t.Status = result;
                    t.SendDt = DateTime.Now;
                    t.TryTimes++;
                    t.ResultMessage = resultMessage;
                });
            }

            return BaseResponseBuilder.Ok;
        }
    }
}