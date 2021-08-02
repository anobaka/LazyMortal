using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbShell : AdbWrapper
    {
        public AdbInput Input { get; }
        internal AdbShell(AdbWrapper prev) : base(prev, "shell")
        {
            Input = new AdbInput(this);
        }
    }
}