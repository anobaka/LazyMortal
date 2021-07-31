using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbShell : AdbWrapper
    {
        internal AdbShell(AdbWrapper prev) : base(prev, "shell")
        {
        }

        public async Task AdbScreenshot(string pathInDevice)
        {
            await Execute($"screencap {pathInDevice}");
        }
    }
}