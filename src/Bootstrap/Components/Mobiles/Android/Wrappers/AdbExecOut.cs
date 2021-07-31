using System;
using System.Collections.Generic;
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
    }
}