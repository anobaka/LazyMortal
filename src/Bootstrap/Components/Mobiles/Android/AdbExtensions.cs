using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Bootstrap.Components.Mobiles.Android.Models.Exceptions;
using Bootstrap.Components.Terminal.Cmd;
using CliWrap;
using Humanizer;

namespace Bootstrap.Components.Mobiles.Android
{
    public static class AdbExtensions
    {
        private static readonly Dictionary<string, AdbDeviceState> StateMappings = new()
        {
            {"offline", AdbDeviceState.Offline},
            {"device", AdbDeviceState.Device},
            {"nodevice", AdbDeviceState.NoDevice},
        };

        public static bool TryParseDeviceState(string state, out AdbDeviceState s)
        {
            return StateMappings.TryGetValue(state, out s);
        }

        public static void ThrowIfError(string output)
        {
            var lastLine = output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim()).Last(a => a.Length > 0);
            if (!lastLine.Equals("Success", StringComparison.OrdinalIgnoreCase))
            {
                var match = Regex.Match(lastLine, @"^Failure \[(?<error>.*)\]$");
                throw new AdbInternalException(
                    match.Success ? ParseAdbInternalError(match.Groups["error"].Value) : AdbInternalError.Error,
                    output);
            }
        }

        public static AdbInternalError ParseAdbInternalError(string adbFailure)
        {
            return Enum.TryParse<AdbInternalError>(adbFailure, true, out var c)
                ? c
                : AdbInternalError.Error;
        }
    }
}