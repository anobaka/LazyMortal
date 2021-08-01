using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Bootstrap.Components.Mobiles.Android.Wrappers;

namespace Bootstrap.Components.Mobiles.Android.Infrastructures
{
    public class Adb : AdbCommon
    {
        public Adb(AdbInvoker adb) : base(new AdbWrapper(adb))
        {
        }

        public async Task<List<AdbDevice>> Devices(bool showLongOutput = false)
        {
            var cmd = "devices";
            if (showLongOutput)
            {
                cmd += " -l";
            }

            var output = await Execute(cmd);
            const string keyword = "List of devices attached";
            var startIndex = output.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (startIndex > -1)
            {
                startIndex += keyword.Length;
            }

            var keyLines = output.Substring(startIndex)
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var devices = keyLines
                .Select(adbOutput =>
                {
                    var segments = adbOutput.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!(segments.Count > 1))
                    {
                        throw new AdbException(AdbExceptionCode.InvalidDevice, $"Adb device output: {adbOutput}");
                    }

                    var serialNumber = segments[0];
                    segments.RemoveAt(0);

                    var stateString = segments[0];
                    segments.RemoveAt(0);
                    if (stateString == "no")
                    {
                        if (segments.Count < 2)
                        {
                            throw new AdbException(AdbExceptionCode.InvalidDevice, $"Adb device output: {adbOutput}");
                        }

                        stateString += segments[1];
                        segments.RemoveAt(0);
                    }

                    if (!AdbExtensions.TryParseDeviceState(stateString, out var s))
                    {
                        throw new AdbException(AdbExceptionCode.InvalidDevice);
                    }

                    var properties = segments
                        .Select(a => a.Split(':', StringSplitOptions.RemoveEmptyEntries))
                        .ToDictionary(a => a[0], a => a[1]);
                    var description = new AdbDeviceDescription(properties);
                    return new AdbDevice(this, serialNumber, s, description);
                }).ToList();
            return devices;
        }

        public async Task<AdbDevice> UseDevice(int transportId)
        {

        }

        public async Task<AdbDevice> UseDevice(string serialNumber)
        {

        }

        public async Task<AdbDevice> UseUsbDevice()
        {

        }

        public async Task<AdbDevice> UseTcpIpDevice()
        {

        }

        public async Task AdbStartServer()
        {
            await Execute("start-server");
        }

        public async Task KillServer()
        {
            await Execute("kill-server");
        }
    }
}