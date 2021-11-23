using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bakabase.OtherWorld.Business.Components.Notification;
using Bootstrap.Components.Notification.Constants;

namespace Bootstrap.Components.Notification.Messages
{
    public abstract class Message
    {
        public int Id { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.ToBeSent;
        public int TryTimes { get; set; }

        public string Sender { get; set; }
        [Required] public string Receiver { get; set; }
        public string Content { get; set; }

        public MessageType Type { get; set; }

        public DateTime? ScheduleDt { get; set; }
        public DateTime? SendDt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateDt { get; set; } = DateTime.Now;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdateDt { get; set; } = DateTime.Now;
    }
}