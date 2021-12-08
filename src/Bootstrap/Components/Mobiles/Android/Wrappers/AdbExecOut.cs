using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Bootstrap.Components.Mobiles.Android.Wrappers.Shell;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbExecOut : AdbShellWrapper
    {
        internal AdbExecOut(AdbWrapper prev) : base(prev, "exec-out")
        {
        }

        public async Task<MemoryStream> ScreenCap()
        {
            var ms = new MemoryStream();
            await Execute("screencap -p", ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}