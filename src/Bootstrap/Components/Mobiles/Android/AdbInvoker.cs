using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Terminal.Cmd;
using CliWrap;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Mobiles.Android
{
    public class AdbInvoker
    {
        private readonly IOptions<AdbOptions> _options;
        private readonly ILogger<AdbInvoker> _logger;

        public AdbInvoker(IOptions<AdbOptions> options, ILogger<AdbInvoker> logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// TBD: Use custom classes instead of <see cref="CliWrap"/>'s <see cref="CommandResult"/> and <see cref="PipeTarget"/>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="outputStream"></param>
        /// <returns></returns>
        public async Task<CommandResult> Run(string command, PipeTarget outputStream)
        {
            _logger.LogInformation($"Before executing adb command: {command}");
            var result = await Cli.Wrap(_options.Value.ExecutablePath)
                .WithArguments(command)
                .WithStandardOutputPipe(outputStream)
                .ExecuteAsync();
            _logger.LogInformation(
                $"Adb command executed with exit code: {result.ExitCode}, elapsed: {result.RunTime:g}");
            result.ThrowAdbExceptionIfInvalid();
            return result;
        }
    }
}