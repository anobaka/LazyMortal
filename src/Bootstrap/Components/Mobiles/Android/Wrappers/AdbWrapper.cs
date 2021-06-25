using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public abstract class AdbWrapper
    {
        private readonly string _baseCommand;
        private readonly AdbInvoker _adb;

        protected AdbWrapper(AdbInvoker adb)
        {
            _adb = adb;
        }

        protected AdbWrapper(AdbWrapper prev, params string[] appendArguments)
        {
            _adb = prev._adb;
            _baseCommand = prev._baseCommand;
            if (appendArguments?.Any() == true)
            {
                if (_baseCommand.IsNotEmpty())
                {
                    _baseCommand += ' ';
                }

                _baseCommand += string.Join(' ', appendArguments);
            }
        }

        protected async Task<string> Run(params string[] arguments)
        {
            var cmd = _baseCommand;
            if (arguments?.Any() == true)
            {
                cmd += " " + string.Join(" ", arguments);
            }

            return await _adb.Run(cmd);
        }
    }
}