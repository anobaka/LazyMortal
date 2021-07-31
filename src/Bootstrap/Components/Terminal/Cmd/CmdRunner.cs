using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Terminal.Cmd
{
    [Obsolete("Use CliWrap instead")]
    public class CmdRunner
    {
        private const string PathEnvKey = "PATH";
        public Task<CmdResult> Run(out Process process, string filename, string arguments, CancellationToken ct)
        {
            // Start the child process.
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = filename
                }
            };

            var userPathEnv = Environment.GetEnvironmentVariable(PathEnvKey, EnvironmentVariableTarget.User);
            if (userPathEnv.IsNotEmpty())
            {
                p.StartInfo.EnvironmentVariables[PathEnvKey] += userPathEnv;
            }

            if (!string.IsNullOrEmpty(arguments)) p.StartInfo.Arguments = arguments;

            process = p;
            try
            {
                // Redirect the output stream of the child process.
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // Read the output stream first and then wait.
                return p.StandardOutput.ReadToEndAsync().ContinueWith(t =>
                {
                    p.WaitForExit();
                    return new CmdResult(p.ExitCode, t.Result);
                }, ct);
            }
            catch (Exception e)
            {
                return Task.FromResult(new CmdResult(-1, e.Message));
            }
        }
    }
}
