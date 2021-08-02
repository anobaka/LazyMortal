using Bootstrap.Components.Mobiles.Android.Infrastructures;

namespace Bootstrap.Components.Mobiles.Android.Wrappers.Shell
{
    public abstract class AdbShellWrapper : AdbWrapper
    {
        public AdbInput Input { get; init; }
        public AdbPm Pm { get; init; }
        public AdbAm Am { get; init; }
        public AdbMonkey Monkey { get; init; }

        protected AdbShellWrapper(AdbWrapper prev, params string[] appendArguments) : base(prev, appendArguments)
        {
            Input = new AdbInput(this);
            Pm = new AdbPm(this);
            Am = new AdbAm(this);
            Monkey = new AdbMonkey(this);
        }
    }
}