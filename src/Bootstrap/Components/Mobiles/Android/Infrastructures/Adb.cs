using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Bootstrap.Components.Mobiles.Android.Models.Exceptions;
using Bootstrap.Components.Mobiles.Android.Wrappers;

namespace Bootstrap.Components.Mobiles.Android.Infrastructures
{
    public class Adb : AdbCommon
    {
        public Adb(AdbInvoker adb) : base(new AdbWrapper(adb))
        {
        }

        #region Adb

        public async Task<List<AdbDevice>> Devices(string? arguments = null)
        {
            var cmd = $"devices {arguments}";
            var output = await Execute(cmd);
            const string keyword = "List of devices attached";
            var startIndex = output.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (startIndex > -1)
            {
                startIndex += keyword.Length;
            }

            var keyLines = output[startIndex..].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var devices = keyLines
                .Select(adbOutput =>
                {
                    var segments = adbOutput.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!(segments.Count > 1))
                    {
                        throw new AdbException(AdbExceptionCode.Error, $"Adb device output: {adbOutput}");
                    }

                    var serialNumber = segments[0];
                    segments.RemoveAt(0);

                    var stateString = segments[0];
                    segments.RemoveAt(0);
                    if (stateString == "no")
                    {
                        if (segments.Count < 2)
                        {
                            throw new AdbException(AdbExceptionCode.Error, $"Adb device output: {adbOutput}");
                        }

                        stateString += segments[1];
                        segments.RemoveAt(0);
                    }

                    if (!AdbExtensions.TryParseDeviceState(stateString, out var state))
                    {
                        throw new AdbException(AdbExceptionCode.Error);
                    }

                    var properties = segments
                        .Select(a => a.Split(':', StringSplitOptions.RemoveEmptyEntries))
                        .ToDictionary(a => a[0], a => a[1]);
                    var description = new AdbDeviceDescription(properties);
                    return new AdbDevice(serialNumber, state, description);
                }).ToList();
            return devices;
        }

        public async Task<string> Version()
        {
            return await Execute("version");
        }

        public async Task StartServer()
        {
            await Execute("start-server");
        }

        public async Task KillServer()
        {
            await Execute("kill-server");
        }

        public async Task Connect(string host, int port)
        {
            await Connect($"{host}:{port}");
        }

        public async Task Connect(string serialNumber)
        {
            var output = await Execute($"connect {serialNumber}");
            if (!output.Contains("connected"))
            {
                throw new AdbInternalException(AdbInternalError.FailedToConnectDevice, output);
            }
        }

        #endregion

        #region Bridges

        public AdbCommon UseDevice(int transportId)
        {
            return new(this, $"-t {transportId}");
        }

        public AdbCommon UseDevice(string serialNumber)
        {
            return new(this, $"-s {serialNumber}");
        }

        public AdbCommon UseUsbDevice()
        {
            return new(this, $"-d");
        }

        public AdbCommon UseTcpIpDevice()
        {
            return new(this, $"-e");
        }

        public AdbCommon WithArguments(string arguments)
        {
            return new(this, arguments);
        }

        #endregion
    }
}