namespace Bootstrap.Components.Mobiles.Android.Modules
{
    public abstract class AdbModule
    {
        protected readonly AdbInvoker Adb;

        protected AdbModule(AdbInvoker adb)
        {
            Adb = adb;
        }
    }
}