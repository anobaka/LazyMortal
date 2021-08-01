using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbExecOut : AdbWrapper
    {
        public AdbExecOut(AdbWrapper prev) : base(prev, "exec-out")
        {
        }

        public async Task<Stream> ScreenCap()
        {
            var ms = new MemoryStream();
            await Execute("screencap -p", ms);
            return ms;
        }
    }
}