using System.Collections.Generic;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Mobiles.Android.Models
{
    public class AdbDeviceDescription : Dictionary<string, string>
    {
        public string Product => this.GetValueSafely("product");
        public string Model => this.GetValueSafely("model");
        public string Device => this.GetValueSafely("device");
        public string TransportId => this.GetValueSafely("transport_id");
        public string Usb => this.GetValueSafely("usb");

        internal AdbDeviceDescription(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }
    }
}