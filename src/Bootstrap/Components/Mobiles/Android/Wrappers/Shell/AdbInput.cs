using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers.Shell
{
    public class AdbInput : AdbWrapper
    {
        public AdbInput(AdbShellWrapper prev) : base(prev, "input")
        {
        }

        public async Task Tap(int x, int y)
        {
            await Execute($"tap {x} {y}");
        }

        public async Task Text(string text)
        {
            await Execute($"text {text}");
        }

        public async Task KeyEvent(int eventId)
        {
            await Execute($"keyevent {eventId}");
        }

        public async Task Swipe(int x1, int y1, int x2, int y2, int? duration = null)
        {
            var cmd = $"swipe {x1} {y1} {x2} {y2}";
            if (duration.HasValue)
            {
                cmd += $" {duration.Value}";
            }

            await Execute(cmd);
        }
    }
}