using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using CliWrap;

namespace Bootstrap.Components.Mobiles.Android.Infrastructures
{
    public class AdbWrapper
    {
        private readonly string _baseCommand;
        private readonly AdbInvoker _adb;

        public AdbWrapper(AdbInvoker adb, params string[] appendArguments)
        {
            _adb = adb;
            if (appendArguments?.Any() == true)
            {
                if (_baseCommand.IsNotEmpty())
                {
                    _baseCommand += ' ';
                }

                _baseCommand += string.Join(' ', appendArguments);
            }
        }

        protected AdbWrapper(AdbWrapper prev, params string[] appendArguments) : this(prev._adb,
            (new string[] {prev._baseCommand}.Concat(appendArguments ?? new string[] { }).ToArray()))
        {
        }

        public async Task<string> ExecuteAndGetOutputString(string arguments)
        {
            var sb = new StringBuilder();
            var result = await Execute(arguments, );
            return sb.ToString();
        }

        public async Task ExecuteAndSaveToStream(string arguments, Stream output)
        {
            await Execute(arguments, PipeTarget.ToStream(output));
        }

        public async Task ExecuteAndSaveToFile(string arguments, string filePath)
        {
            await Execute(arguments, PipeTarget.ToFile(filePath));
        }

        public async Task<CommandResult> Execute(string arguments, Stream stream)
        {
            return await Execute(new[] {arguments}, stream);
        }

        public async Task<CommandResult> Execute(string[] arguments, Stream stream)
        {
            var cmd = _baseCommand;
            if (arguments?.Any() == true)
            {
                cmd += " " + string.Join(" ", arguments);
            }

            return await _adb.Run(cmd, stream);
        }
    }
}