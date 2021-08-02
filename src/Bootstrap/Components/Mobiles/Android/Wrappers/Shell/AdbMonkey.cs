using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers.Shell
{
    public class AdbMonkey : AdbWrapper
    {
        internal AdbMonkey(AdbShellWrapper prev) : base(prev, "monkey")
        {
        }

        public async Task LaunchApp(string packageName)
        {
            await Execute($"-p {packageName} -c android.intent.category.LAUNCHER 1");
        }
    }
}