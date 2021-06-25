using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Terminal.Cmd;
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

        public async Task<string> Run(string command)
        {
            _logger.LogInformation($"Before executing adb command: {command}");
            var result = await new CmdRunner().Run(out _, _options.Value.ExecutablePath, command, new CancellationToken());
            _logger.LogInformation($"Adb command executed with exit code: {result.ExitCode}, output: {result.Output}");
            result.ThrowAdbExceptionIfInvalid();
            return result.Output;
        }
    }
}