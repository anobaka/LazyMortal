using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Microsoft.Extensions.Hosting;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class Adb : AdbCommon
    {
        private List<AdbDevice> _devices = new();
        private readonly IHostEnvironment _env;

        public Adb(AdbInvoker adb, IHostEnvironment env) : base(adb, env)
        {
            _env = env;
        }

        public async Task<List<AdbDevice>> GetDevices(bool refresh)
        {
            if (refresh)
            {
                await RefreshDevices();
            }

            return _devices;
        }

        public async Task<AdbDevice> GetDevice(int index)
        {
            if (_devices.Count <= index)
            {
                await RefreshDevices();
            }

            return _devices[index];
        }

        public async Task<AdbDevice> GetDevice(string serialNumber)
        {
            var device = _devices.FirstOrDefault(t => t.SerialNumber == serialNumber);
            if (device == null)
            {
                await RefreshDevices();
            }

            return _devices.FirstOrDefault(t => t.SerialNumber == serialNumber);
        }

        public async Task AdbStartServer()
        {
            await Run("start-server");
        }

        public async Task AdbKillServer()
        {
            await Run("kill-server");
        }

        public async Task RefreshDevices()
        {
            var output = await Run("devices -l");
            const string keyword = "List of devices attached";
            var startIndex = output.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (startIndex > -1)
            {
                startIndex += keyword.Length;
            }

            var keyLines = output.Substring(startIndex)
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            _devices = keyLines
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
                    return new AdbDevice(_env, serialNumber, s, description, this);
                }).ToList();
        }
    }
}