using System;

namespace Bootstrap.Components.Notification.Implementations.Email
{
    [Obsolete]
    public class EmailOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DefaultFromAddress { get; set; }
    }
}