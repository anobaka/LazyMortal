using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bootstrap.Components.Notification.Abstractions.Models.Constants;

namespace Bootstrap.Components.Notification.Abstractions.Models.Entities
{
    public record Message
    {
        public int Id { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.ToBeSent;
        public int TryTimes { get; set; }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public string RawDataString { get; set; }
        [NotMapped] public object RawData { get; set; }

        public NotificationType Type { get; set; }

        public DateTime? ScheduleDt { get; set; }
        public DateTime? SendDt { get; set; }

        /// <summary>
        /// todo: db generation
        /// </summary>
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateDt { get; set; } = DateTime.Now;

        /// <summary>
        /// todo: db generation
        /// </summary>
        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdateDt { get; set; } = DateTime.Now;
        public string ResultMessage { get; set; }
    }
}