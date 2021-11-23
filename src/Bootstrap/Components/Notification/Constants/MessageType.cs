using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bakabase.OtherWorld.Business.Components.Notification
{
    [Flags]
    public enum MessageType
    {
        Os = 1,
        Email = 2,
        WeChat = 4,
        Sms = 8,
        OsAndEmail = Os | Email
    }
}