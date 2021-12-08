using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers.Shell
{
    public class AdbWm : AdbWrapper
    {
        public AdbWm(AdbShellWrapper prev) : base(prev, "wm")
        {
        }

        public async Task<Size> Size()
        {
            var output = await Execute("size");
            var match = Regex.Match(output, @"(?<w>\d+)x(?<h>\d+)");
            return new Size(int.Parse(match.Groups["w"].Value), int.Parse(match.Groups["h"].Value));
        }

        public async Task<int> Density()
        {
            var output = await Execute("density");
            return int.Parse(output);
        }
    }
}