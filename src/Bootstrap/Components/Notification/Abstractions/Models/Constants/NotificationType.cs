using System;

namespace Bootstrap.Components.Notification.Abstractions.Models.Constants
{
    [Flags]
    public enum NotificationType
    {
        Os = 1,
        Email = 2,
        WeChat = 4,
        Sms = 8,
        OsAndEmail = Os | Email
    }
}