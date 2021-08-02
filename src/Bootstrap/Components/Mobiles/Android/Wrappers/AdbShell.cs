using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Bootstrap.Components.Mobiles.Android.Wrappers.Shell;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbShell : AdbShellWrapper
    {
        internal AdbShell(AdbWrapper prev) : base(prev, "shell")
        {
        }
    }
}