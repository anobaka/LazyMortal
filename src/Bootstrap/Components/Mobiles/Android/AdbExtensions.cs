using System.Collections.Generic;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Bootstrap.Components.Terminal.Cmd;
using CliWrap;

namespace Bootstrap.Components.Mobiles.Android
{
    public static class AdbExtensions
    {
        private static readonly Dictionary<string, AdbDeviceState> StateMappings = new()
        {
            { "offline", AdbDeviceState.Offline },
            { "device", AdbDeviceState.Device },
            { "nodevice", AdbDeviceState.NoDevice },
        };
        public static void ThrowAdbExceptionIfInvalid(this CommandResult r)
        {
            if (r.ExitCode != 0)
            {
                throw new AdbException(r.ExitCode);
            }
        }

        public static bool TryParseDeviceState(string state, out AdbDeviceState s)
        {
            return StateMappings.TryGetValue(state, out s);
        }
    }
}