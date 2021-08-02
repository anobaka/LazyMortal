using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers.Shell
{
    public class AdbAm : AdbWrapper
    {
        public AdbAm(AdbShellWrapper prev) : base(prev, "am")
        {
        }
    }
}