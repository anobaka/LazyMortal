using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models.Constants;

namespace Bootstrap.Components.Mobiles.Android.Models
{
    public class AdbDevice
    {
        public string SerialNumber { get; }
        public AdbDeviceState State { get; }
        public AdbDeviceDescription Description { get; }

        public AdbDevice(string serialNumber, AdbDeviceState state, AdbDeviceDescription description = null)
        {
            SerialNumber = serialNumber;
            State = state;
            Description = description;
        }
    }
}