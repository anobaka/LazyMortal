using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Bootstrap.Components.Mobiles.Android.Models;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Microsoft.Extensions.Hosting;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbDevice : AdbCommon
    {
        public string SerialNumber { get; }
        public AdbDeviceState State { get; }
        public AdbDeviceDescription Description { get; }

        internal AdbDevice(AdbWrapper prev, string serialNumber, AdbDeviceState state,
            AdbDeviceDescription description) : base(prev,"-t", description.TransportId)
        {
            SerialNumber = serialNumber;
            State = state;
            Description = description;
        }
    }
}