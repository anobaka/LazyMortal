using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Bootstrap.Components.Mobiles.Android.Models.Exceptions;
using Bootstrap.Components.Terminal.Cmd;
using Bootstrap.Extensions;
using CliWrap;
using CliWrap.Exceptions;
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

            var errorSb = new StringBuilder();
            await using var outputMs = new MemoryStream();
            var consoleOutputPipeline = PipeTarget.ToStream(outputMs, true);

            var result = await Cli.Wrap(_options.Value.ExecutablePath)
                .WithArguments(command)
                .WithStandardOutputPipe(PipeTarget.Merge(outputStream, consoleOutputPipeline))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(errorSb))
                .ExecuteAsync();

            _logger.LogInformation(
                $"Adb command executed with exit code: {result.ExitCode}, elapsed: {result.RunTime:g}");

            var error = errorSb.Length > 0 ? errorSb.ToString().Trim() : null;

            if (result.ExitCode != 0)
            {
                var output = error;
                if (outputMs.Length > 0)
                {
                    outputMs.Seek(0, SeekOrigin.Begin);
                    using var sr = new StreamReader(outputMs);
                    var info = await sr.ReadToEndAsync();
                    if (output.IsNotEmpty())
                    {
                        output = info + Environment.NewLine + error;
                    }
                    else
                    {
                        output = info;
                    }
                }

                throw new AdbInvalidExitCodeException(result.ExitCode, output);
            }

            if (error != null)
            {
                _logger.LogWarning(error);
            }

            return result;
        }
    }
}