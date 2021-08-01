using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbInput: AdbWrapper
    {
        public AdbInput(AdbWrapper prev) : base(prev, "input")
        {
        }

        public async Task AdbTap(int x, int y)
        {
            await Execute($"tap {x} {y}");
        }

        public async Task AdbText(string text)
        {
            await Execute($"text {text}");
        }

        public async Task AdbKeyEvent(int eventId)
        {
            await Execute($"keyevent {eventId}");
        }

        public async Task AdbSwipe(int x1, int y1, int x2, int y2)
        {
            await Execute($"swipe {x1} {y1} {x2} {y2}");
        }
    }
}
