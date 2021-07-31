using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Bootstrap.Components.Mobiles.Android.Wrappers;

namespace Bootstrap.Components.Mobiles.Android.Infrastructures
{
    public class Adb
    {
        private List<AdbDevice> _devices;
        private readonly AdbCommon _adbCommon;

        public Adb(AdbInvoker adb)
        {
            _adbCommon = new AdbCommon(new AdbWrapper(adb));
        }

        public async Task<List<AdbDevice>> GetDevices(bool forceRefresh = false)
        {
            if (_devices == null || forceRefresh)
            {
                await RefreshDevices();
            }

            return _devices;
        }

        public async Task<AdbDevice> GetDevice(int index)
        {
            if (_devices == null || _devices.Count <= index)
            {
                await RefreshDevices();
            }

            if (_devices.Count <= index)
            {
                throw new AdbException(AdbExceptionCode.InvalidDevice, $"{nameof(index)}:{index} is out of range.");
            }

            return _devices[index];
        }

        public async Task<AdbDevice> GetDevice(string serialNumber)
        {
            var device = _devices?.FirstOrDefault(t => t.SerialNumber == serialNumber);
            if (device == null)
            {
                await RefreshDevices();
                device = _devices.FirstOrDefault(a => a.SerialNumber == serialNumber);
                if (device == null)
                {
                    throw new AdbException(AdbExceptionCode.InvalidDevice, $"Device:{serialNumber} is not found.");
                }
            }

            return device;
        }

        public async Task AdbStartServer()
        {
            await _adbCommon.Execute("start-server");
        }

        public async Task AdbKillServer()
        {
            await _adbCommon.Execute("kill-server");
        }

        public async Task RefreshDevices()
        {
            var output = await _adbCommon.Execute("devices -l");
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
                    return new AdbDevice(_adbCommon, serialNumber, s, description);
                }).ToList();
        }
    }
}