using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers.Shell
{
    public class AdbAm : AdbWrapper
    {
        public AdbAm(AdbShellWrapper prev) : base(prev, "am")
        {
        }

        public async Task Broadcast(string action, string arguments = null)
        {
            await Execute($"broadcast -a {action} {arguments}");
        }

        public async Task ForceStop(string packageName)
        {
            await Execute($"force-stop {packageName}");
        }
    }
}