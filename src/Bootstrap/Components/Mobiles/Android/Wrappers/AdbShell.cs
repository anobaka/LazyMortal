using System.Threading.Tasks;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbShell : AdbWrapper
    {
        internal AdbShell(AdbWrapper prev, params string[] appendArguments) : base(prev, appendArguments)
        {
        }

        public async Task AdbScreenshot(string pathInDevice)
        {
            await Run($"screencap {pathInDevice}");
        }
    }
}