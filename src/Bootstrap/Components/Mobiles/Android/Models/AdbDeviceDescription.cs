using System.Collections.Generic;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Mobiles.Android.Models
{
    public class AdbDeviceDescription : Dictionary<string, string>
    {
        public string? Product => this.GetValueOrDefault("product");
        public string? Model => this.GetValueOrDefault("model");
        public string? Device => this.GetValueOrDefault("device");
        public string? TransportId => this.GetValueOrDefault("transport_id");
        public string? Usb => this.GetValueOrDefault("usb");

        internal AdbDeviceDescription(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }
    }
}